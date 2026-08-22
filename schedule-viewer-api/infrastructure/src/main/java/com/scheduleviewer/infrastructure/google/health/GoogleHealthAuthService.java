package com.scheduleviewer.infrastructure.google.health;

import com.google.api.client.auth.oauth2.Credential;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.util.List;

/** Google Health API OAuth and credential access. */
@Service
public class GoogleHealthAuthService {

    static final String TOKEN_FOLDER = "token_GoogleHealth";
    static final List<String> SCOPES = List.of(
            "https://www.googleapis.com/auth/googlehealth.activity_and_fitness.readonly",
            "https://www.googleapis.com/auth/googlehealth.health_metrics_and_measurements.readonly",
            "https://www.googleapis.com/auth/googlehealth.sleep.readonly",
            "https://www.googleapis.com/auth/googlehealth.profile.readonly");

    private final GoogleAuthService googleAuthService;

    public GoogleHealthAuthService(GoogleAuthService googleAuthService) {
        this.googleAuthService = googleAuthService;
    }

    public String initialize() throws Exception {
        return googleAuthService.startAuthFlowAndGetUrl(SCOPES, TOKEN_FOLDER, null);
    }

    public String reauthorize() throws Exception {
        return googleAuthService.startAuthFlowAndGetUrl(SCOPES, TOKEN_FOLDER, null, true);
    }

    public boolean hasToken() {
        return googleAuthService.hasToken(TOKEN_FOLDER);
    }

    public Credential getCredential() throws Exception {
        Credential credential = googleAuthService.loadCredential(SCOPES, TOKEN_FOLDER);
        if (credential == null) return null;

        Long expiresIn = credential.getExpiresInSeconds();
        if (expiresIn != null && expiresIn <= 60 && credential.getRefreshToken() != null
                && !credential.refreshToken()) {
            throw new IOException("Google Health access token could not be refreshed.");
        }
        return credential;
    }
}
