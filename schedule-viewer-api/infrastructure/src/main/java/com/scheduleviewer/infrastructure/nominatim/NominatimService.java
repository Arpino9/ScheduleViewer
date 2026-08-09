package com.scheduleviewer.infrastructure.nominatim;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.util.UriComponentsBuilder;

import java.net.URI;
import java.nio.charset.StandardCharsets;
import java.text.Normalizer;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * Nominatim (OpenStreetMap) サービス
 * <p>.NET版の NominatimReader に相当</p>
 */
@Service
public class NominatimService {

    private static final Logger log = LoggerFactory.getLogger(NominatimService.class);
    private static final String USER_AGENT    = "ScheduleViewerApp/1.0";
    private static final long MIN_REQUEST_INTERVAL_MILLIS = 1_000L;
    private static final Pattern POSTAL_CODE_PATTERN = Pattern.compile("(\\d{3})[^\\d]?(\\d{4})");

    private final RestTemplate restTemplate;
    private final String searchUrl;
    private final ObjectMapper mapper = new ObjectMapper();
    private final ConcurrentHashMap<String, Optional<Coordinates>> geocodeCache = new ConcurrentHashMap<>();
    private final Object requestLock = new Object();
    private long lastRequestStartedAt;

    public NominatimService(
            RestTemplate restTemplate,
            @Value("${scheduleviewer.nominatim.base-url:https://nominatim.openstreetmap.org}") String baseUrl) {
        this.restTemplate = restTemplate;
        this.searchUrl = baseUrl.replaceAll("/+$", "") + "/search";
    }

    /**
     * 住所から地図タイル画像URLを取得する
     */
    public String getMapTileUrl(String address) throws Exception {
        double[] latLon = geocode(address);
        if (latLon == null) return null;
        int[] tile = latLonToTile(latLon[0], latLon[1], 14);
        return "https://tile.openstreetmap.org/14/%d/%d.png".formatted(tile[0], tile[1]);
    }

    /**
     * 住所から都道府県・市区町村を取得する
     */
    public String getTownArea(String address) throws Exception {
        URI url = UriComponentsBuilder.fromHttpUrl(searchUrl)
                .queryParam("q", address)
                .queryParam("format", "json")
                .queryParam("addressdetails", "1")
                .queryParam("limit", "1")
                .build()
                .encode(StandardCharsets.UTF_8)
                .toUri();

        String json = fetchWithUserAgent(url);
        JsonNode root = mapper.readTree(json);
        if (!root.isArray() || root.size() == 0) return "";

        JsonNode addr = root.get(0).get("address");
        String prefecture = addr.path("state").asText("");
        String city = !addr.path("city").isMissingNode()   ? addr.get("city").asText() :
                      !addr.path("town").isMissingNode()   ? addr.get("town").asText() :
                      !addr.path("village").isMissingNode() ? addr.get("village").asText() : "";
        String suburb = addr.path("suburb").asText("");

        return prefecture + city + suburb;
    }

    /**
     * 住所から緯度経度を取得する (ジオコーディング)
     *
     * @return [latitude, longitude] または null
     */
    public double[] geocode(String address) throws Exception {
        if (address == null || address.isBlank()) return null;

        String normalizedAddress = address.trim();
        Optional<Coordinates> cached = geocodeCache.get(normalizedAddress);
        if (cached != null) return cached.map(Coordinates::toArray).orElse(null);

        // Nominatim public API policy: single-threaded, at most one request per second.
        synchronized (requestLock) {
            cached = geocodeCache.get(normalizedAddress);
            if (cached != null) return cached.map(Coordinates::toArray).orElse(null);

            Optional<Coordinates> result = searchCoordinates(normalizedAddress);
            if (result.isEmpty()) {
                Matcher postalCode = POSTAL_CODE_PATTERN.matcher(
                        Normalizer.normalize(normalizedAddress, Normalizer.Form.NFKC));
                if (postalCode.find()) {
                    result = searchCoordinates(postalCode.group(1) + "-" + postalCode.group(2) + " Japan");
                }
            }
            geocodeCache.put(normalizedAddress, result);
            return result.map(Coordinates::toArray).orElse(null);
        }
    }

    private Optional<Coordinates> searchCoordinates(String query) throws Exception {
        long waitMillis = MIN_REQUEST_INTERVAL_MILLIS
                - (System.currentTimeMillis() - lastRequestStartedAt);
        if (waitMillis > 0) Thread.sleep(waitMillis);
        lastRequestStartedAt = System.currentTimeMillis();

        URI url = UriComponentsBuilder.fromHttpUrl(searchUrl)
                .queryParam("q", query)
                .queryParam("format", "json")
                .queryParam("limit", "1")
                .build()
                .encode(StandardCharsets.UTF_8)
                .toUri();
        JsonNode root = mapper.readTree(fetchWithUserAgent(url));
        if (!root.isArray() || root.isEmpty()) return Optional.empty();
        return Optional.of(new Coordinates(
                root.get(0).get("lat").asDouble(),
                root.get(0).get("lon").asDouble()));
    }

    /**
     * 緯度・経度をタイル座標 (x, y) に変換する
     */
    public int[] latLonToTile(double lat, double lon, int zoom) {
        int x = (int) Math.floor((lon + 180.0) / 360.0 * Math.pow(2, zoom));
        int y = (int) Math.floor(
                (1.0 - Math.log(Math.tan(lat * Math.PI / 180.0) + 1.0 / Math.cos(lat * Math.PI / 180.0)) / Math.PI)
                / 2.0 * Math.pow(2, zoom));
        return new int[]{x, y};
    }

    private String fetchWithUserAgent(URI url) {
        org.springframework.http.HttpHeaders headers = new org.springframework.http.HttpHeaders();
        headers.set("User-Agent", USER_AGENT);
        var entity = new org.springframework.http.HttpEntity<>(headers);
        var response = restTemplate.exchange(url, org.springframework.http.HttpMethod.GET, entity, String.class);
        return response.getBody();
    }

    private record Coordinates(double latitude, double longitude) {
        double[] toArray() {
            return new double[]{latitude, longitude};
        }
    }
}
