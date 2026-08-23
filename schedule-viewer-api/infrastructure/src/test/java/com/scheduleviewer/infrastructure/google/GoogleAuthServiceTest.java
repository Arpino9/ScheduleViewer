package com.scheduleviewer.infrastructure.google;

import org.junit.jupiter.api.Test;

import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.assertEquals;

class GoogleAuthServiceTest {

    @Test
    void explicitCredentialHomeTakesPriority() {
        assertEquals(
                Path.of("configured-home", ".scheduleviewer", "token_Calendar"),
                GoogleAuthService.resolveTokenDirectory(
                        "token_Calendar", "configured-home", "user-profile", "jvm-home"));
    }

    @Test
    void windowsUserProfileIsUsedWhenCredentialHomeIsNotConfigured() {
        assertEquals(
                Path.of("user-profile", ".scheduleviewer", "token_Calendar"),
                GoogleAuthService.resolveTokenDirectory(
                        "token_Calendar", "", "user-profile", "jvm-home"));
    }

    @Test
    void jvmUserHomeRemainsTheCrossPlatformFallback() {
        assertEquals(
                Path.of("jvm-home", ".scheduleviewer", "token_Calendar"),
                GoogleAuthService.resolveTokenDirectory(
                        "token_Calendar", null, null, "jvm-home"));
    }
}
