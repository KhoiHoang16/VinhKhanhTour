window.mapInterop = {
    map: null,
    marker: null,
    initMap: function (elementId, dotNetRef, initialLat, initialLng) {
        // If map was already initialized, clean it up
        if (this.map !== null) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }

        // Default to Ho Chi Minh City if null
        var lat = initialLat || 10.762622;
        var lng = initialLng || 106.660172;

        // Initialize Map
        this.map = L.map(elementId).setView([lat, lng], 15);

        // Add OSM TileLayer
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        }).addTo(this.map);

        // Create draggable marker
        this.marker = L.marker([lat, lng], { draggable: true }).addTo(this.map);

        var self = this;

        // On map click
        this.map.on('click', function (e) {
            self.marker.setLatLng(e.latlng);
            dotNetRef.invokeMethodAsync('UpdateCoordinates', e.latlng.lat, e.latlng.lng);
        });

        // On marker drag
        this.marker.on('dragend', function (e) {
            var position = self.marker.getLatLng();
            dotNetRef.invokeMethodAsync('UpdateCoordinates', position.lat, position.lng);
        });
        
        // Fix a known issue where Leaflet maps don't render correctly inside Modals
        // Trigger a resize after a short delay so the map draws its tiles.
        setTimeout(function() {
            if (self.map) {
                self.map.invalidateSize();
            }
        }, 100);
    }
};
