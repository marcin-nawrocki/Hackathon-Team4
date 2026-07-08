var DashboardMap = (function () {
    var map = null;
    var layers = {};

    var COLORS = {
        glow: '#7cb342',
        high: '#7cb342',
        highStroke: '#ffffff',
        low: '#9ccc65',
        lowStroke: '#ffffff'
    };

    function createGlowMarker(feature, latlng) {
        var isHigh = feature.properties.score === 'high';
        var group = L.featureGroup();

        if (isHigh) {
            L.circleMarker(latlng, {
                radius: 22,
                fillColor: COLORS.glow,
                fillOpacity: 0.1,
                stroke: false,
                interactive: false
            }).addTo(group);

            L.circleMarker(latlng, {
                radius: 14,
                fillColor: COLORS.glow,
                fillOpacity: 0.2,
                stroke: false,
                interactive: false
            }).addTo(group);

            L.circleMarker(latlng, {
                radius: 8,
                fillColor: COLORS.glow,
                fillOpacity: 0.35,
                stroke: false,
                interactive: false
            }).addTo(group);
        }

        L.circleMarker(latlng, {
            radius: isHigh ? 5 : 3.5,
            fillColor: isHigh ? COLORS.high : COLORS.low,
            color: isHigh ? COLORS.highStroke : COLORS.lowStroke,
            weight: isHigh ? 1.5 : 1,
            fillOpacity: isHigh ? 0.95 : 0.75,
            className: isHigh ? 'map-marker-high' : 'map-marker-low'
        }).addTo(group);

        return group;
    }

    function bindPopup(feature, layer) {
        var p = feature.properties;

        if (p.bookings != null) {
            layer.bindPopup(
                '<strong>' + p.country + '</strong><br>' +
                'Bookings: ' + p.bookings.toLocaleString()
            );
            return;
        }

        var changeClass = p.change >= 0 ? 'positive' : 'negative';
        var changeSign = p.change >= 0 ? '+' : '';
        layer.bindPopup(
            '<strong>' + p.name + '</strong><br>' +
            'Bookings: ' + value
        );
    }

    // Choropleth fill color scaled by booking volume for a country.
    function getChoroplethColor(value) {
        return value >= 10000 ? '#99f6e4' :
               value >= 4000  ? '#5eead4' :
               value >= 2000  ? '#2dd4bf' :
               value >= 1000  ? '#14b8a6' :
               value > 0      ? '#0d9488' :
                                'transparent';
    }

    // Normalizes a country name so DB values and polygon names can be matched.
    function normalizeCountry(name) {
        var key = (name || '').toString().trim().toLowerCase();
        var aliases = {
            'usa': 'united states of america',
            'us': 'united states of america',
            'united states': 'united states of america',
            'america': 'united states of america',
            'uk': 'united kingdom',
            'gb': 'united kingdom',
            'great britain': 'united kingdom',
            'england': 'united kingdom',
            'scotland': 'united kingdom',
            'wales': 'united kingdom',
            'russia': 'russia',
            'russian federation': 'russia',
            'south korea': 'south korea',
            'republic of korea': 'south korea',
            'korea': 'south korea',
            'czech republic': 'czechia',
            'uae': 'united arab emirates',
            'holland': 'netherlands',
            'the netherlands': 'netherlands',
            'serbia': 'republic of serbia'
        };
        return aliases.hasOwnProperty(key) ? aliases[key] : key;
    }

    // Rolls the real Top Locations rows up to a per-country total keyed by normalized name.
    function aggregateByCountry(locations) {
        var agg = {};
        (locations || []).forEach(function (loc) {
            var key = normalizeCountry(loc.name);
            if (!key) return;
            if (!agg[key]) agg[key] = { name: loc.name, value: 0 };
            agg[key].value += loc.value || 0;
        });
        return agg;
    }

    // Caches the fetched country-polygon GeoJSON so live refreshes only restyle.
    var countryPolygons = null;

    // Highlights whole countries (polygons) shaded by real booking volume.
    // countriesUrl must resolve to a GeoJSON of country polygons whose
    // feature.properties.name is the country's English name.
    function renderCountryHighlights(name, countriesUrl, locations) {
        if (!map) return Promise.reject('Map not initialized');

        var polygonsPromise = countryPolygons
            ? Promise.resolve(countryPolygons)
            : fetch(countriesUrl).then(function (r) { return r.json(); }).then(function (data) {
                countryPolygons = data;
                return data;
            });

        return polygonsPromise.then(function (countries) {
            drawCountryHighlights(name, countries, locations);
            return layers[name];
        });
    }

    function drawCountryHighlights(name, countries, locations) {
        var agg = aggregateByCountry(locations);

        function dataFor(feature) {
            return agg[normalizeCountry(feature.properties && feature.properties.name)] || null;
        }

        function styleFor(feature) {
            var data = dataFor(feature);
            var value = data ? data.value : 0;
            return {
                fillColor: getChoroplethColor(value),
                fillOpacity: value > 0 ? 0.78 : 0,
                color: value > 0 ? COLORS.glow : 'rgba(255,255,255,0.06)',
                weight: value > 0 ? 1 : 0.5,
                opacity: value > 0 ? 0.9 : 0.25
            };
        }

        removeLayer(name);

        var layer = L.geoJSON(countries, {
            style: styleFor,
            onEachFeature: function (feature, lyr) {
                var data = dataFor(feature);
                if (!data) return;

                lyr.bindPopup(
                    '<strong>' + data.name + '</strong><br>' +
                    'Bookings: ' + Math.round(data.value).toLocaleString()
                );

                lyr.on({
                    mouseover: function (e) {
                        e.target.setStyle({ weight: 2, fillOpacity: 0.92, color: COLORS.highStroke });
                        e.target.bringToFront();
                    },
                    mouseout: function (e) { layer.resetStyle(e.target); }
                });
            }
        });

        layer.addTo(map);
        layers[name] = layer;
        return layer;
    }

    function init(containerId, options) {
        options = options || {};

        map = L.map(containerId, {
            center: options.center || [20, 0],
            zoom: options.zoom != null ? options.zoom : 2,
            zoomControl: false,
            scrollWheelZoom: false,
            worldCopyJump: true,
            minZoom: 2,
            maxZoom: 8
        });

        L.tileLayer('https://{s}.basemaps.cartocdn.com/light_nolabels/{z}/{x}/{y}{r}.png', {
            maxZoom: 19,
            subdomains: 'abcd',
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> &copy; <a href="https://carto.com/attributions">CARTO</a>'
        }).addTo(map);

        L.control.zoom({ position: 'bottomright' }).addTo(map);

        return map;
    }

    function addGeoJsonLayer(name, url, customOptions) {
        if (!map) return Promise.reject('Map not initialized');

        var options = customOptions || {};
        var layer = L.geoJSON(null, {
            pointToLayer: function (feature, latlng) {
                if (options.pointToLayer) return options.pointToLayer(feature, latlng);
                return createGlowMarker(feature, latlng);
            },
            onEachFeature: options.onEachFeature || bindPopup,
            style: options.polygonStyle || undefined
        });

        layer.addTo(map);
        layers[name] = layer;

        return fetch(url)
            .then(function (response) { return response.json(); })
            .then(function (data) {
                layer.addData(data);
                if (options.fitToData && layer.getBounds().isValid()) {
                    map.fitBounds(layer.getBounds(), { padding: [40, 40], maxZoom: options.fitMaxZoom || 4 });
                }
                return layer;
            });
    }

    function addGeoJsonData(name, data, customOptions) {
        if (!map) return null;

        var options = customOptions || {};
        var layer = L.geoJSON(data, {
            pointToLayer: function (feature, latlng) {
                if (options.pointToLayer) return options.pointToLayer(feature, latlng);
                return createGlowMarker(feature, latlng);
            },
            onEachFeature: options.onEachFeature || bindPopup,
            style: options.polygonStyle || undefined
        });

        layer.addTo(map);
        layers[name] = layer;
        return layer;
    }

    function getLayer(name) {
        return layers[name] || null;
    }

    function removeLayer(name) {
        if (layers[name] && map) {
            map.removeLayer(layers[name]);
            delete layers[name];
        }
    }

    return {
        init: init,
        addGeoJsonLayer: addGeoJsonLayer,
        addGeoJsonData: addGeoJsonData,
        renderCountryHighlights: renderCountryHighlights,
        getLayer: getLayer,
        removeLayer: removeLayer,
        getMap: function () { return map; },
        invalidateSize: function () { if (map) map.invalidateSize(); }
    };
})();
