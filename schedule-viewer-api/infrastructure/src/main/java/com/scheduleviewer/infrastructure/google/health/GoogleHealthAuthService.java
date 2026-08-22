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
    static final String IMPORT_TOKEN_FOLDER = "token_GoogleHealthImport";
    static final List<String> SCOPES = List.of(
            "https://www.googleapis.com/auth/googlehealth.activity_and_fitness.readonly",
            "https://www.googleapis.com/auth/googlehealth.health_metrics_and_measurements.readonly",
            "https://www.googleapis.com/auth/googlehealth.sleep.readonly",
            "https://www.googleapis.com/auth/googlehealth.profile.readonly");
    static final List<String> IMPORT_SCOPES = List.of(
            "https://www.googleapis.com/auth/googlehealth.activity_and_fitness.writeonly",
            "https://www.googleapis.com/auth/googlehealth.health_metrics_and_measurements.writeonly",
            "https://www.googleapis.com/auth/googlehealth.sleep.writeonly");

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

    public String initializeImport() throws Exception {
        return googleAuthService.startAuthFlowAndGetUrl(IMPORT_SCOPES, IMPORT_TOKEN_FOLDER, null);
    }

    public String reauthorizeImport() throws Exception {
        return googleAuthService.startAuthFlowAndGetUrl(IMPORT_SCOPES, IMPORT_TOKEN_FOLDER, null, true);
    }

    public boolean hasImportToken() {
        return googleAuthService.hasToken(IMPORT_TOKEN_FOLDER);
    }

    public Credential getImportCredential() throws Exception {
        return loadAndRefresh(IMPORT_SCOPES, IMPORT_TOKEN_FOLDER);
    }

    public Credential getCredential() throws Exception {
        return loadAndRefresh(SCOPES, TOKEN_FOLDER);
    }

    private Credential loadAndRefresh(List<String> scopes, String tokenFolder) throws Exception {
        Credential credential = googleAuthService.loadCredential(scopes, tokenFolder);
        if (credential == null) return null;

        Long expiresIn = credential.getExpiresInSeconds();
        if (expiresIn != null && expiresIn <= 60 && credential.getRefreshToken() != null
                && !credential.refreshToken()) {
            throw new IOException("Google Health access token could not be refreshed.");
        }
        return credential;
    }
}
