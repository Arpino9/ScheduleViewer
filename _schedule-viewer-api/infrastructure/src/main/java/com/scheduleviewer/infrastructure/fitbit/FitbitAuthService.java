package com.scheduleviewer.infrastructure.fitbit;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.scheduleviewer.infrastructure.config.AppProperties;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.awt.Desktop;
import java.io.IOException;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.URI;
import java.net.URLDecoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.SecureRandom;
import java.util.Base64;
import java.util.concurrent.CompletableFuture;

/**
 * Fitbit OAuth2 PKCE 認証サービス
 * <p>.NET版の FitbitBase に相当。PKCE フローを実装する。</p>
 */
@Service
public class FitbitAuthService {

    private static final Logger log = LoggerFactory.getLogger(FitbitAuthService.class);

    private static final String AUTH_URI = "https://www.fitbit.com/oauth2/authorize";
    private static final String[] SCOPES = {
        "activity", "profile", "weight", "heartrate", "sleep",
        "nutrition", "settings", "location", "oxygen_saturation"
    };

    private final AppProperties props;
    private final FitbitTokenStore tokenStore;
    private final ObjectMapper objectMapper = new ObjectMapper();

    public FitbitAuthService(AppProperties props, FitbitTokenStore tokenStore) {
        this.props      = props;
        this.tokenStore = tokenStore;
    }

    /**
     * PKCE フローを開始し、認証URLを返す。
     * コールバック受信後のトークン取得はバックグラウンドで行う。
     */
    public String initialize() throws Exception {
        String codeVerifier  = generateCodeVerifier();
        String codeChallenge = generateCodeChallenge(codeVerifier);
        String clientId      = props.getFitbit().getClientId();
        String redirectUri   = props.getFitbit().getRedirectUri();
        String scope         = String.join("+", SCOPES);

        String authUrl =
            AUTH_URI + "?client_id=" + clientId +
            "&response_type=code" +
            "&redirect_uri=" + redirectUri +
            "&scope=" + scope +
            "&code_challenge=" + codeChallenge +
            "&code_challenge_method=S256";

        log.info("Fitbit認証URL: {}", authUrl);

        // コールバック受信をバックグラウンドで待機
        Thread.ofVirtual().start(() -> {
            try {
                String code = waitForCallback();
                if (code == null) {
                    log.error("認証コードの取得に失敗しました");
                    return;
                }
                fetchTokensFromCode(code, codeVerifier);
                if (tokenStore.hasToken()) {
                    log.info("Fitbit認証完了");
                }
            } catch (Exception e) {
                log.error("Fitbit認証失敗", e);
            }
        });

        return authUrl;
    }

    /**
     * リフレッシュトークンでアクセストークンを再取得する
     */
    public void refreshAccessToken() throws Exception {
        String body = "grant_type=refresh_token" +
                      "&refresh_token=" + tokenStore.getRefreshToken() +
                      "&client_id=" + props.getFitbit().getClientId();
        fetchTokens(body);
    }

    /**
     * ローカルサーバーでコールバックを待機する (localhost:5000)
     */
    private String waitForCallback() throws Exception {
        URI redirectUri = URI.create(props.getFitbit().getRedirectUri());
        int port = redirectUri.getPort() > 0 ? redirectUri.getPort() : 5000;

        try (var serverSocket = new ServerSocket(port)) {
            log.info("コールバック待機中: port={}", port);
            try (var clientSocket = serverSocket.accept()) {
                var request = new String(clientSocket.getInputStream().readNBytes(4096));

                // GET /?code=XXXXX HTTP/1.1 からcodeを抽出
                String code = null;
                for (String line : request.split("\r\n")) {
                    if (line.startsWith("GET ")) {
                        String path = line.split(" ")[1];
                        if (path.contains("code=")) {
                            code = URLDecoder.decode(
                                path.substring(path.indexOf("code=") + 5).split("&")[0],
                                StandardCharsets.UTF_8);
                        }
                        break;
                    }
                }

                // レスポンスを返す
                String html = "<html><body>認証成功。このウィンドウは閉じてください。</body></html>";
                String response = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n" + html;
                clientSocket.getOutputStream().write(response.getBytes(StandardCharsets.UTF_8));

                return code;
            }
        }
    }

    private void fetchTokensFromCode(String code, String codeVerifier) throws Exception {
        String body = "client_id="  + props.getFitbit().getClientId() +
                      "&grant_type=authorization_code" +
                      "&redirect_uri=" + props.getFitbit().getRedirectUri() +
                      "&code=" + code +
                      "&code_verifier=" + codeVerifier;
        fetchTokens(body);
    }

    private void fetchTokens(String body) throws Exception {
        var requestBuilder = HttpRequest.newBuilder()
                .uri(URI.create(props.getFitbit().getTokenRequestUri()))
                .header("Content-Type", "application/x-www-form-urlencoded");

        // client_secret がある場合のみ Basic 認証。PKCE (public app) では不要
        String clientSecret = props.getFitbit().getClientSecret();
        if (clientSecret != null && !clientSecret.isEmpty()) {
            String credentials = props.getFitbit().getClientId() + ":" + clientSecret;
            String basicAuth   = Base64.getEncoder().encodeToString(credentials.getBytes(StandardCharsets.UTF_8));
            requestBuilder.header("Authorization", "Basic " + basicAuth);
        }

        var request = requestBuilder
                .POST(HttpRequest.BodyPublishers.ofString(body))
                .build();

        var response = HttpClient.newHttpClient().send(request, HttpResponse.BodyHandlers.ofString());

        if (response.statusCode() != 200) {
            log.error("トークン取得失敗: {}", response.body());
            return;
        }

        JsonNode json = objectMapper.readTree(response.body());
        tokenStore.store(
                json.get("access_token").asText(),
                json.get("refresh_token").asText(),
                json.get("expires_in").asLong());

        log.debug("トークン取得成功");
    }

    // PKCE ユーティリティ
    private String generateCodeVerifier() {
        byte[] bytes = new byte[96];
        new SecureRandom().nextBytes(bytes);
        return Base64.getUrlEncoder().withoutPadding().encodeToString(bytes);
    }

    private String generateCodeChallenge(String codeVerifier) throws Exception {
        byte[] digest = MessageDigest.getInstance("SHA-256")
                .digest(codeVerifier.getBytes(StandardCharsets.US_ASCII));
        return Base64.getUrlEncoder().withoutPadding().encodeToString(digest);
    }
}
