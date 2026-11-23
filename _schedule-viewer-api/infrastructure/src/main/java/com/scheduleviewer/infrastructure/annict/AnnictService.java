package com.scheduleviewer.infrastructure.annict;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.scheduleviewer.domain.entity.AnimeEntity;
import com.scheduleviewer.infrastructure.config.AppProperties;
import com.scheduleviewer.infrastructure.google.spreadsheet.SpreadsheetService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.StringJoiner;

/**
 * Annict GraphQL API サービス
 * <p>.NET版の AnnictReader に相当</p>
 */
@Service
public class AnnictService {

    private static final Logger log = LoggerFactory.getLogger(AnnictService.class);
    private static final String GRAPHQL_URL = "https://api.annict.com/graphql";

    private final AppProperties props;
    private final SpreadsheetService spreadsheetService;
    private final ObjectMapper mapper = new ObjectMapper();

    public AnnictService(AppProperties props, SpreadsheetService spreadsheetService) {
        this.props = props;
        this.spreadsheetService = spreadsheetService;
    }

    /**
     * タイトルでアニメ情報を検索する
     *
     * @param title     タイトル
     * @param first     取得件数
     * @param castFirst キャスト取得件数
     */
    public List<AnimeEntity> fetch(String title, int first, int castFirst) throws Exception {
        String query = """
                query ($titles: [String!], $first: Int, $castFirst: Int) {
                  searchWorks(titles: $titles, first: $first) {
                    nodes {
                      title
                      seasonName
                      seasonYear
                      officialSiteUrl
                      wikipediaUrl
                      episodesCount
                      image {
                        recommendedImageUrl
                        twitterAvatarUrl
                      }
                      casts(first: $castFirst, orderBy: { field: SORT_NUMBER, direction: ASC }) {
                        nodes {
                          name
                          character { name }
                          person { name }
                        }
                      }
                    }
                  }
                }
                """;

        String variables = """
                {"titles": ["%s"], "first": %d, "castFirst": %d}
                """.formatted(title.replace("\"", "\\\""), first, castFirst);

        String payload = """
                {"query": %s, "variables": %s}
                """.formatted(toJsonString(query), variables);

        var request = HttpRequest.newBuilder()
                .uri(URI.create(GRAPHQL_URL))
                .header("Authorization", "Bearer " + props.getAnnict().getToken())
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(payload, StandardCharsets.UTF_8))
                .build();

        var response = HttpClient.newHttpClient().send(request, HttpResponse.BodyHandlers.ofString());
        log.debug("Annict レスポンス: {}", response.body());

        return parseResponse(response.body());
    }

    public List<AnimeEntity> fetch(String title) throws Exception {
        return fetch(title, 5, 10);
    }

    private List<AnimeEntity> parseResponse(String json) throws Exception {
        JsonNode root   = mapper.readTree(json);
        JsonNode nodes  = root.path("data").path("searchWorks").path("nodes");

        List<AnimeEntity> result = new ArrayList<>();
        for (JsonNode node : nodes) {
            String cast      = convertCasts(node.path("casts").path("nodes"));
            String thumbnail = node.path("image").path("recommendedImageUrl").asText("");
            String title     = node.path("title").asText("");

            // スプレッドシートの「取得(番組)」シートから概要を取得 (Annict登録有無にかかわらず優先)
            String caption = spreadsheetService.findCaptionByTitle(title);
            if (caption == null || caption.isEmpty()) {
                caption = node.path("synopsis").asText("");
            }

            result.add(new AnimeEntity(
                    true,
                    title,
                    node.path("seasonName").asText(""),
                    node.path("seasonYear").asText(""),
                    node.path("officialSiteUrl").asText(""),
                    node.path("wikipediaUrl").asText(""),
                    node.path("episodesCount").asText(""),
                    cast,
                    thumbnail,
                    "", "", "",
                    caption));
        }
        return result;
    }

    private String convertCasts(JsonNode casts) {
        StringJoiner joiner = new StringJoiner("／");
        for (JsonNode cast : casts) {
            String character  = cast.path("character").path("name").asText("");
            String voiceActor = cast.path("person").path("name").asText("");
            joiner.add(character + ":" + voiceActor);
        }
        return joiner.toString();
    }

    private String toJsonString(String s) {
        return "\"" + s.replace("\\", "\\\\").replace("\"", "\\\"")
                        .replace("\n", "\\n").replace("\r", "\\r") + "\"";
    }
}
