var DashboardMap = (function () {
    var map = null;
    var layers = {};

    var COLORS = {
        glow: '#22d3ee',
        high: '#99f6e4',
        highStroke: '#ccfbf1',
        low: '#2dd4bf',
        lowStroke: '#115e59'
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
        var changeClass = p.change >= 0 ? 'positive' : 'negative';
        var changeSign = p.change >= 0 ? '+' : '';
        layer.bindPopup(
            '<strong>' + p.name + '</strong> (' + p.country + ')<br>' +
            'Value: ' + p.value.toLocaleString() + '<br>' +
            'Change: <span class="' + changeClass + '">' + changeSign + p.change + '%</span>'
        );
    }

    function init(containerId, options) {
        options = options || {};

        map = L.map(containerId, {
            center: options.center || [48, 12],
            zoom: options.zoom || 3,
            zoomControl: false,
            scrollWheelZoom: false,
            worldCopyJump: true
        });

        L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_nolabels/{z}/{x}/{y}{r}.png', {
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
                if (layer.getBounds().isValid()) {
                    map.fitBounds(layer.getBounds(), { padding: [40, 40], maxZoom: 4 });
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
        getLayer: getLayer,
        removeLayer: removeLayer,
        getMap: function () { return map; },
        invalidateSize: function () { if (map) map.invalidateSize(); }
    };
})();
