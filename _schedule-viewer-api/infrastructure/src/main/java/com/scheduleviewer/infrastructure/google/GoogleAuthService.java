package com.scheduleviewer.infrastructure.google;

import com.google.api.client.auth.oauth2.AuthorizationCodeRequestUrl;
import com.google.api.client.auth.oauth2.Credential;
import com.google.api.client.extensions.java6.auth.oauth2.AuthorizationCodeInstalledApp;
import com.google.api.client.extensions.jetty.auth.oauth2.LocalServerReceiver;
import com.google.api.client.googleapis.auth.oauth2.GoogleAuthorizationCodeFlow;
import com.google.api.client.googleapis.auth.oauth2.GoogleClientSecrets;
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
import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.TimeUnit;

/**
 * Google OAuth2 認証サービス (共通基盤)
 * <p>.NET版の GoogleServiceBase&lt;TService&gt; に相当</p>
 */
@Service
public class GoogleAuthService {

    private static final org.slf4j.Logger log = org.slf4j.LoggerFactory.getLogger(GoogleAuthService.class);
    private static final GsonFactory JSON_FACTORY = GsonFactory.getDefaultInstance();
    private static final String APPLICATION_NAME  = "ScheduleViewer";

    private final AppProperties props;

    public GoogleAuthService(AppProperties props) {
        this.props = props;
    }

    /**
     * OAuth2 認証を行い Credential を取得する
     *
     * @param scopes          必要なスコープ
     * @param tokenFolderName トークン保存フォルダ名
     */
    public Credential authorize(List<String> scopes, String tokenFolderName) throws Exception {
        NetHttpTransport transport = GoogleNetHttpTransport.newTrustedTransport();

        GoogleClientSecrets secrets;
        try (var stream = new FileInputStream(props.getGoogle().getClientSecretPath());
             var reader = new InputStreamReader(stream)) {
            secrets = GoogleClientSecrets.load(JSON_FACTORY, reader);
        }

        var tokenDir = Paths.get(System.getProperty("user.home"), ".scheduleviewer", tokenFolderName).toFile();
        var flow = new GoogleAuthorizationCodeFlow.Builder(transport, JSON_FACTORY, secrets, scopes)
                .setDataStoreFactory(new FileDataStoreFactory(tokenDir))
                .setAccessType("offline")
                .build();

        return new AuthorizationCodeInstalledApp(flow, new LocalServerReceiver()).authorize("user");
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
     * 指定サービスの OAuth トークンが保存済みか確認する
     * <p>起動時のブロッキングを避けるため、@PostConstruct から呼び出す</p>
     */
    /**
     * OAuth認証フローを開始し、認証URLを返す。認証完了後に onAuthComplete を実行する。
     * すでに認証済みの場合は null を返す。
     */
    public String startAuthFlowAndGetUrl(List<String> scopes, String tokenFolderName, Runnable onAuthComplete) throws Exception {
        NetHttpTransport transport = GoogleNetHttpTransport.newTrustedTransport();

        GoogleClientSecrets secrets;
        try (var stream = new FileInputStream(props.getGoogle().getClientSecretPath());
             var reader = new InputStreamReader(stream)) {
            secrets = GoogleClientSecrets.load(JSON_FACTORY, reader);
        }

        var tokenDir = Paths.get(System.getProperty("user.home"), ".scheduleviewer", tokenFolderName).toFile();
        var flow = new GoogleAuthorizationCodeFlow.Builder(transport, JSON_FACTORY, secrets, scopes)
                .setDataStoreFactory(new FileDataStoreFactory(tokenDir))
                .setAccessType("offline")
                .build();

        // すでに認証済みか確認
        var existing = flow.loadCredential("user");
        if (existing != null && existing.getRefreshToken() != null) {
            return null;
        }

        var urlFuture = new CompletableFuture<String>();
        var receiver = new LocalServerReceiver();

        var app = new AuthorizationCodeInstalledApp(flow, receiver) {
            @Override
            protected void onAuthorization(AuthorizationCodeRequestUrl authorizationUrl) {
                // ブラウザを開かず、URLをフロントエンドに返す
                urlFuture.complete(authorizationUrl.build());
            }
        };

        Thread.ofVirtual().start(() -> {
            try {
                app.authorize("user");
                log.info("OAuth認証完了: {}", tokenFolderName);
                if (onAuthComplete != null) {
                    onAuthComplete.run();
                }
            } catch (Exception e) {
                log.error("OAuth認証失敗: {}", tokenFolderName, e);
            }
        });

        return urlFuture.get(15, TimeUnit.SECONDS);
    }

    public boolean hasToken(String tokenFolderName) {
        var tokenFile = Paths.get(
                System.getProperty("user.home"), ".scheduleviewer", tokenFolderName, "StoredCredential");
        try {
            if (!Files.exists(tokenFile) || Files.size(tokenFile) < 100) return false;
            // 空の HashMap = 82 bytes。実際のトークンは 300+ bytes になる
            return true;
        } catch (Exception e) {
            return false;
        }
    }
}
