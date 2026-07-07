var DashboardMap = (function () {
    var map = null;
    var layers = {};

    function pointStyle(feature) {
        var isHigh = feature.properties.score === 'high';
        return {
            radius: isHigh ? 9 : 6,
            fillColor: isHigh ? '#c4e830' : '#374151',
            color: isHigh ? '#c4e830' : '#4b5563',
            weight: 1,
            opacity: 1,
            fillOpacity: isHigh ? 0.85 : 0.6
        };
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
            center: options.center || [50, 10],
            zoom: options.zoom || 4,
            zoomControl: false,
            scrollWheelZoom: false
        });

        L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);

        L.control.zoom({ position: 'bottomright' }).addTo(map);

        return map;
    }

    function addGeoJsonLayer(name, url, customOptions) {
        if (!map) return Promise.reject('Map not initialized');

        var options = customOptions || {};
        var layer = L.geoJSON(null, {
            pointToLayer: function (feature, latlng) {
                return L.circleMarker(latlng, options.style ? options.style(feature) : pointStyle(feature));
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
                    map.fitBounds(layer.getBounds(), { padding: [30, 30], maxZoom: 6 });
                }
                return layer;
            });
    }

    function addGeoJsonData(name, data, customOptions) {
        if (!map) return null;

        var options = customOptions || {};
        var layer = L.geoJSON(data, {
            pointToLayer: function (feature, latlng) {
                return L.circleMarker(latlng, options.style ? options.style(feature) : pointStyle(feature));
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
        getMap: function () { return map; }
    };
})();
