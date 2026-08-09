package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.nominatim.NominatimService;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

/** Address geocoding for calendar event maps. */
@RestController
@RequestMapping("/api/map")
public class MapController {

    private final NominatimService nominatimService;

    public MapController(NominatimService nominatimService) {
        this.nominatimService = nominatimService;
    }

    @GetMapping("/geocode")
    public MapLocation geocode(@RequestParam String address) throws Exception {
        if (address.isBlank()) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "address is required");
        }

        double[] coordinates = nominatimService.geocode(address);
        if (coordinates == null) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "address was not found");
        }
        return new MapLocation(coordinates[0], coordinates[1]);
    }

    public record MapLocation(double latitude, double longitude) {}
}
