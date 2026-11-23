package com.scheduleviewer.infrastructure.google.spreadsheet;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class SpreadsheetServiceTest {

    @Test
    void matchesAchievementWhenSheetNameIsMissingClosingBracket() {
        assertTrue(SpreadsheetService.achievementMatches(
                "忘却の彼方 [Lost to the Ages]",
                "【The Elder Scrolls V: Skyrim Special Edition】\n"
                        + "“忘却の彼方”のクエストを完了 [Complete \"Lost to the Ages\"]",
                "The Elder Scrolls V: Skyrim Special Edition",
                "忘却の彼方 [Lost to the Ages"));
    }

    @Test
    void doesNotMatchWhenGameTitleIsAbsentFromDescription() {
        assertFalse(SpreadsheetService.achievementMatches(
                "忘却の彼方 [Lost to the Ages]",
                "【タイトル】\n別のゲーム",
                "The Elder Scrolls V: Skyrim Special Edition",
                "忘却の彼方 [Lost to the Ages"));
    }

    @Test
    void doesNotMatchWhenAchievementNameIsAbsentFromEventTitle() {
        assertFalse(SpreadsheetService.achievementMatches(
                "別の実績",
                "【タイトル】\nThe Elder Scrolls V: Skyrim Special Edition",
                "The Elder Scrolls V: Skyrim Special Edition",
                "忘却の彼方 [Lost to the Ages"));
    }
}
