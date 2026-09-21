package com.scheduleviewer.infrastructure.google;

import com.google.api.client.auth.oauth2.AuthorizationCodeRequestUrl;
import com.google.api.client.auth.oauth2.Credential;
import com.google.api.client.extensions.java6.auth.oauth2.AuthorizationCodeInstalledApp;
import com.google.api.client.extensions.jetty.auth.oauth2.LocalServerReceiver;
import com.google.api.client.googleapis.auth.oauth2.GoogleAuthorizationCodeFlow;
import com.google.api.client.googleapis.auth.oauth2.GoogleClientSecrets;
import com.google.api.client.googleapis.auth.oauth2.GoogleTokenResponse;
import com.google.api.client.googleapis.javanet.GoogleNetHttpTransport;
import com.google.api.client.http.javanet.NetHttpTransport;
import com.google.api.client.json.gson.GsonFactory;
import com.google.api.client.util.store.FileDataStoreFactory;
import com.scheduleviewer.infrastructure.config.AppProperties;
import org.springframework.stereotype.Service;

import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.security.SecureRandom;
import java.time.Duration;
import java.time.Instant;
import java.util.Base64;
import java.util.List;
import java.util.Map;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.TimeUnit;

/**
 * Google OAuth2 認証サービス (共通基盤)
 * <p>ローカルでは一時ポート、Azureでは固定HTTPSコールバックを使用する。</p>
 */
@Service
public class GoogleAuthService {

    private static final org.slf4j.Logger log = org.slf4j.LoggerFactory.getLogger(GoogleAuthService.class);
    private static final GsonFactory JSON_FACTORY = GsonFactory.getDefaultInstance();
    private static final String APPLICATION_NAME = "ScheduleViewer";
    private static final Duration PENDING_AUTH_TTL = Duration.ofMinutes(10);

    private final AppProperties props;
    private final SecureRandom secureRandom = new SecureRandom();
    private final Map<String, PendingAuthorization> pendingAuthorizations = new ConcurrentHashMap<>();

    public GoogleAuthService(AppProperties props) {
        this.props = props;
    }

    /**
     * OAuth2 Credentialを取得する。
     * Azureでは保存済みCredentialだけを返し、未認証時にローカル待受を起動しない。
     */
    public Credential authorize(List<String> scopes, String tokenFolderName) throws Exception {
        var flow = createFlow(scopes, tokenFolderName);
        var existing = flow.loadCredential("user");
        if (existing != null) {
            return existing;
        }

        if (isWebCallbackMode()) {
            throw new IllegalStateException(
                    "Google OAuth is not completed for " + tokenFolderName
                    + ". Start authorization from /api/auth/google/{service}.");
        }

        return new AuthorizationCodeInstalledApp(flow, newLocalServerReceiver()).authorize("user");
    }

    /** Loads a stored credential without starting an interactive browser flow. */
    public Credential loadCredential(List<String> scopes, String tokenFolderName) throws Exception {
        return createFlow(scopes, tokenFolderName).loadCredential("user");
    }

    public NetHttpTransport newTransport() throws Exception {
        return GoogleNetHttpTransport.newTrustedTransport();
    }

    public GsonFactory getJsonFactory() {
        return JSON_FACTORY;
    }

    public String getApplicationName() {
        return APPLICATION_NAME;
    }

    /**
     * OAuth認証フローを開始し、認証URLを返す。
     * すでに認証済みの場合はnullを返す。
     */
    public String startAuthFlowAndGetUrl(
            List<String> scopes,
            String tokenFolderName,
            Runnable onAuthComplete) throws Exception {
        return startAuthFlowAndGetUrl(scopes, tokenFolderName, onAuthComplete, false);
    }

    /** Starts OAuth, optionally discarding the stored credential first. */
    public String startAuthFlowAndGetUrl(
            List<String> scopes,
            String tokenFolderName,
            Runnable onAuthComplete,
            boolean forceReauthorization) throws Exception {
        var flow = createFlow(scopes, tokenFolderName);

        if (forceReauthorization) {
            flow.getCredentialDataStore().delete("user");
        }

        var existing = flow.loadCredential("user");
        if (existing != null && existing.getRefreshToken() != null) {
            return null;
        }

        if (isWebCallbackMode()) {
            removeExpiredPendingAuthorizations();

            String state = createState();
            pendingAuthorizations.put(
                    state,
                    new PendingAuthorization(
                            List.copyOf(scopes),
                            tokenFolderName,
                            onAuthComplete,
                            Instant.now()));

            var authorizationUrl = flow.newAuthorizationUrl()
                    .setRedirectUri(props.getGoogle().getRedirectUri())
                    .setState(state);
            if (forceReauthorization) {
                authorizationUrl.setApprovalPrompt("force");
            }
            return authorizationUrl.build();
        }

        var urlFuture = new CompletableFuture<String>();
        var receiver = newLocalServerReceiver();
        var app = new AuthorizationCodeInstalledApp(flow, receiver) {
            @Override
            protected void onAuthorization(AuthorizationCodeRequestUrl authorizationUrl) {
                urlFuture.complete(authorizationUrl.build());
            }
        };

        Thread.ofVirtual().start(() -> {
            try {
                app.authorize("user");
                log.info("OAuth認証完了: {}", tokenFolderName);
                runCompletionCallback(onAuthComplete, tokenFolderName);
            } catch (Exception e) {
                log.error("OAuth認証失敗: {}", tokenFolderName, e);
            }
        });

        return urlFuture.get(15, TimeUnit.SECONDS);
    }

    /**
     * Azureの固定HTTPSコールバックで認可コードをトークンへ交換して保存する。
     *
     * @return 認証を完了したトークンフォルダー名
     */
    public String completeWebAuthorization(String code, String state) throws Exception {
        if (!isWebCallbackMode()) {
            throw new IllegalStateException("GOOGLE_REDIRECT_URI is not configured.");
        }
        if (isBlank(code) || isBlank(state)) {
            throw new IllegalArgumentException("Google OAuth callback requires code and state.");
        }

        var pending = pendingAuthorizations.remove(state);
        if (pending == null) {
            throw new IllegalArgumentException("Google OAuth state is invalid or has expired.");
        }
        if (pending.createdAt().plus(PENDING_AUTH_TTL).isBefore(Instant.now())) {
            throw new IllegalArgumentException("Google OAuth state has expired. Start authorization again.");
        }

        var flow = createFlow(pending.scopes(), pending.tokenFolderName());
        GoogleTokenResponse tokenResponse = flow.newTokenRequest(code)
                .setRedirectUri(props.getGoogle().getRedirectUri())
                .execute();
        flow.createAndStoreCredential(tokenResponse, "user");

        log.info("OAuth認証完了: {}", pending.tokenFolderName());
        runCompletionCallback(pending.onAuthComplete(), pending.tokenFolderName());
        return pending.tokenFolderName();
    }

    private GoogleAuthorizationCodeFlow createFlow(
            List<String> scopes,
            String tokenFolderName) throws Exception {
        NetHttpTransport transport = GoogleNetHttpTransport.newTrustedTransport();
        GoogleClientSecrets secrets = loadClientSecrets();

        var tokenDir = tokenBasePath()
                .resolve(tokenFolderName)
                .toFile();

        return new GoogleAuthorizationCodeFlow.Builder(
                transport,
                JSON_FACTORY,
                secrets,
                scopes)
                .setDataStoreFactory(new FileDataStoreFactory(tokenDir))
                .setAccessType("offline")
                .build();
    }

    private GoogleClientSecrets loadClientSecrets() throws Exception {
        var google = props.getGoogle();
        boolean hasClientId = !isBlank(google.getClientId());
        boolean hasClientSecret = !isBlank(google.getClientSecret());

        if (hasClientId || hasClientSecret) {
            if (!hasClientId || !hasClientSecret) {
                throw new IllegalStateException(
                        "GOOGLE_CLIENT_ID and GOOGLE_CLIENT_SECRET must both be configured.");
            }

            var details = new GoogleClientSecrets.Details()
                    .setClientId(google.getClientId())
                    .setClientSecret(google.getClientSecret());
            return new GoogleClientSecrets().setWeb(details);
        }

        if (isBlank(google.getClientSecretPath())) {
            throw new IllegalStateException(
                    "Configure GOOGLE_CLIENT_ID and GOOGLE_CLIENT_SECRET, "
                    + "or GOOGLE_CLIENT_SECRET_PATH for local development.");
        }

        try (var stream = new FileInputStream(google.getClientSecretPath());
             var reader = new InputStreamReader(stream)) {
            return GoogleClientSecrets.load(JSON_FACTORY, reader);
        }
    }

    private boolean isWebCallbackMode() {
        return !isBlank(props.getGoogle().getRedirectUri());
    }

    private String createState() {
        byte[] bytes = new byte[32];
        secureRandom.nextBytes(bytes);
        return Base64.getUrlEncoder().withoutPadding().encodeToString(bytes);
    }

    private void removeExpiredPendingAuthorizations() {
        Instant cutoff = Instant.now().minus(PENDING_AUTH_TTL);
        pendingAuthorizations.entrySet().removeIf(
                entry -> entry.getValue().createdAt().isBefore(cutoff));
    }

    private void runCompletionCallback(Runnable callback, String tokenFolderName) {
        if (callback == null) {
            return;
        }
        try {
            callback.run();
        } catch (Exception e) {
            log.error("認証後のデータ読み込みに失敗: {}", tokenFolderName, e);
        }
    }

    private LocalServerReceiver newLocalServerReceiver() {
        return new LocalServerReceiver.Builder()
                .setPort(-1)
                .build();
    }

    public boolean hasToken(String tokenFolderName) {
        var tokenFile = tokenBasePath()
                .resolve(tokenFolderName)
                .resolve("StoredCredential");
        try {
            if (!Files.exists(tokenFile) || Files.size(tokenFile) < 100) {
                return false;
            }
            return true;
        } catch (Exception e) {
            return false;
        }
    }

    private java.nio.file.Path tokenBasePath() {
        String configuredPath = props.getGoogle().getTokenBasePath();
        if (!isBlank(configuredPath)) {
            return Paths.get(configuredPath);
        }
        if (isWebCallbackMode()) {
            return Paths.get("/home/data/.scheduleviewer");
        }
        return Paths.get(System.getProperty("user.home"), ".scheduleviewer");
    }

    private static boolean isBlank(String value) {
        return value == null || value.isBlank();
    }

    private record PendingAuthorization(
            List<String> scopes,
            String tokenFolderName,
            Runnable onAuthComplete,
            Instant createdAt) {
    }
}
