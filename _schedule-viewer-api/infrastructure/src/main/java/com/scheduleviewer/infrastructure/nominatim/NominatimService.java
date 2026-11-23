package com.scheduleviewer.infrastructure.nominatim;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.util.UriComponentsBuilder;

/**
 * Nominatim (OpenStreetMap) サービス
 * <p>.NET版の NominatimReader に相当</p>
 */
@Service
public class NominatimService {

    private static final Logger log = LoggerFactory.getLogger(NominatimService.class);
    private static final String NOMINATIM_URL = "https://nominatim.openstreetmap.org/search";
    private static final String USER_AGENT    = "ScheduleViewerApp/1.0";

    private final RestTemplate restTemplate;
    private final ObjectMapper mapper = new ObjectMapper();

    public NominatimService(RestTemplate restTemplate) {
        this.restTemplate = restTemplate;
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
        String url = UriComponentsBuilder.fromHttpUrl(NOMINATIM_URL)
                .queryParam("q", address)
                .queryParam("format", "json")
                .queryParam("addressdetails", "1")
                .queryParam("limit", "1")
                .toUriString();

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
        String url = UriComponentsBuilder.fromHttpUrl(NOMINATIM_URL)
                .queryParam("q", address)
                .queryParam("format", "json")
                .toUriString();

        String json = fetchWithUserAgent(url);
        JsonNode root = mapper.readTree(json);
        if (!root.isArray() || root.size() == 0) return null;

        double lat = root.get(0).get("lat").asDouble();
        double lon = root.get(0).get("lon").asDouble();
        return new double[]{lat, lon};
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

    private String fetchWithUserAgent(String url) {
        org.springframework.http.HttpHeaders headers = new org.springframework.http.HttpHeaders();
        headers.set("User-Agent", USER_AGENT);
        var entity = new org.springframework.http.HttpEntity<>(headers);
        var response = restTemplate.exchange(url, org.springframework.http.HttpMethod.GET, entity, String.class);
        return response.getBody();
    }
}
