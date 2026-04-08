// ===== POI MAP (existing — unchanged) =====
window.mapInterop = {
    map: null,
    marker: null,
    initMap: function (elementId, dotNetRef, initialLat, initialLng) {
        if (this.map !== null) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }

        var lat = initialLat || 10.762622;
        var lng = initialLng || 106.660172;

        this.map = L.map(elementId).setView([lat, lng], 15);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        }).addTo(this.map);

        this.marker = L.marker([lat, lng], { draggable: true }).addTo(this.map);

        var self = this;

        this.map.on('click', function (e) {
            self.marker.setLatLng(e.latlng);
            dotNetRef.invokeMethodAsync('UpdateCoordinates', e.latlng.lat, e.latlng.lng);
        });

        this.marker.on('dragend', function (e) {
            var position = self.marker.getLatLng();
            dotNetRef.invokeMethodAsync('UpdateCoordinates', position.lat, position.lng);
        });

        setTimeout(function () {
            if (self.map) {
                self.map.invalidateSize();
            }
        }, 100);
    }
};

// ===== TOUR MAP (new) =====
window.tourMapInterop = {
    map: null,
    startMarker: null,
    endMarker: null,
    routeLine: null,
    poiMarkers: [],
    dotNetRef: null,
    clickMode: 'start', // 'start' | 'end'

    // Các icon tùy chỉnh
    _createIcon: function (color, label) {
        return L.divIcon({
            className: 'tour-marker-icon',
            html: '<div style="background:' + color + ';color:#fff;width:32px;height:32px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-weight:bold;font-size:14px;box-shadow:0 2px 8px rgba(0,0,0,0.4);border:3px solid #fff;">' + label + '</div>',
            iconSize: [32, 32],
            iconAnchor: [16, 16]
        });
    },

    _createPoiIcon: function (index) {
        return L.divIcon({
            className: 'tour-poi-icon',
            html: '<div style="background:#f59e0b;color:#fff;width:26px;height:26px;border-radius:50%;display:flex;align-items:center;justify-content:center;font-weight:bold;font-size:12px;box-shadow:0 2px 6px rgba(0,0,0,0.3);border:2px solid #fff;">' + index + '</div>',
            iconSize: [26, 26],
            iconAnchor: [13, 13]
        });
    },

    initTourMap: function (elementId, dotNetRef, startLat, startLng, endLat, endLng) {
        // Cleanup
        if (this.map !== null) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }

        this.dotNetRef = dotNetRef;
        this.poiMarkers = [];
        this.clickMode = 'start';

        // Center tại Vĩnh Khánh
        var centerLat = startLat || 10.7607;
        var centerLng = startLng || 106.7030;

        this.map = L.map(elementId).setView([centerLat, centerLng], 16);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        }).addTo(this.map);

        var self = this;

        // Nếu đã có start/end (edit mode)
        if (startLat && startLng && startLat !== 0) {
            this.startMarker = L.marker([startLat, startLng], {
                icon: this._createIcon('#22c55e', 'A'),
                draggable: true
            }).addTo(this.map).bindPopup('📍 Điểm bắt đầu');

            this.startMarker.on('dragend', function () {
                var pos = self.startMarker.getLatLng();
                dotNetRef.invokeMethodAsync('UpdateStartPoint', pos.lat, pos.lng);
                self._tryCalculateRoute();
            });

            this.clickMode = 'end';
        }

        if (endLat && endLng && endLat !== 0) {
            this.endMarker = L.marker([endLat, endLng], {
                icon: this._createIcon('#ef4444', 'B'),
                draggable: true
            }).addTo(this.map).bindPopup('🏁 Điểm kết thúc');

            this.endMarker.on('dragend', function () {
                var pos = self.endMarker.getLatLng();
                dotNetRef.invokeMethodAsync('UpdateEndPoint', pos.lat, pos.lng);
                self._tryCalculateRoute();
            });

            this.clickMode = 'start'; // Reset after placing both
        }

        // Click handler — đặt marker
        this.map.on('click', function (e) {
            if (self.clickMode === 'start') {
                // Đặt / di chuyển Start marker
                if (self.startMarker) {
                    self.startMarker.setLatLng(e.latlng);
                } else {
                    self.startMarker = L.marker(e.latlng, {
                        icon: self._createIcon('#22c55e', 'A'),
                        draggable: true
                    }).addTo(self.map).bindPopup('📍 Điểm bắt đầu').openPopup();

                    self.startMarker.on('dragend', function () {
                        var pos = self.startMarker.getLatLng();
                        dotNetRef.invokeMethodAsync('UpdateStartPoint', pos.lat, pos.lng);
                        self._tryCalculateRoute();
                    });
                }
                dotNetRef.invokeMethodAsync('UpdateStartPoint', e.latlng.lat, e.latlng.lng);
                self.clickMode = 'end';
            } else {
                // Đặt / di chuyển End marker
                if (self.endMarker) {
                    self.endMarker.setLatLng(e.latlng);
                } else {
                    self.endMarker = L.marker(e.latlng, {
                        icon: self._createIcon('#ef4444', 'B'),
                        draggable: true
                    }).addTo(self.map).bindPopup('🏁 Điểm kết thúc').openPopup();

                    self.endMarker.on('dragend', function () {
                        var pos = self.endMarker.getLatLng();
                        dotNetRef.invokeMethodAsync('UpdateEndPoint', pos.lat, pos.lng);
                        self._tryCalculateRoute();
                    });
                }
                dotNetRef.invokeMethodAsync('UpdateEndPoint', e.latlng.lat, e.latlng.lng);
                self.clickMode = 'start';

                // Cả 2 marker đã có → tính route
                self._tryCalculateRoute();
            }
        });

        // Nếu edit mode và cả 2 marker đã sẵn → tính route
        if (this.startMarker && this.endMarker) {
            this._tryCalculateRoute();
        }

        setTimeout(function () {
            if (self.map) self.map.invalidateSize();
        }, 100);
    },

    // Gọi OSRM API để tính route
    _tryCalculateRoute: function () {
        if (!this.startMarker || !this.endMarker) return;

        var startPos = this.startMarker.getLatLng();
        var endPos = this.endMarker.getLatLng();

        var url = 'https://router.project-osrm.org/route/v1/driving/'
            + startPos.lng + ',' + startPos.lat + ';'
            + endPos.lng + ',' + endPos.lat
            + '?overview=full&geometries=geojson';

        var self = this;

        fetch(url)
            .then(function (response) { return response.json(); })
            .then(function (data) {
                if (data.code !== 'Ok' || !data.routes || data.routes.length === 0) {
                    console.error('OSRM routing failed:', data);
                    return;
                }

                var route = data.routes[0];
                var coordinates = route.geometry.coordinates; // [lng, lat] pairs
                var distanceKm = Math.round((route.distance / 1000) * 100) / 100;

                // Xóa route cũ
                if (self.routeLine) {
                    self.map.removeLayer(self.routeLine);
                }

                // Vẽ route mới
                var latLngs = coordinates.map(function (c) { return [c[1], c[0]]; });
                self.routeLine = L.polyline(latLngs, {
                    color: '#f59e0b',
                    weight: 5,
                    opacity: 0.8,
                    dashArray: '10, 8'
                }).addTo(self.map);

                // Fit bounds để hiện toàn bộ route
                self.map.fitBounds(self.routeLine.getBounds(), { padding: [40, 40] });

                // Gửi route coordinates về Blazor
                // Chuyển thành mảng [lat, lng] cho C#
                var routePoints = coordinates.map(function (c) {
                    return { lat: c[1], lng: c[0] };
                });

                self.dotNetRef.invokeMethodAsync('OnRouteCalculated', JSON.stringify(routePoints), distanceKm);
            })
            .catch(function (err) {
                console.error('Route calculation error:', err);
            });
    },

    // Hiển thị POI markers trên map (gọi từ Blazor sau khi detect POI)
    showPoiMarkers: function (poisJson) {
        var self = this;

        // Xóa markers cũ
        this.poiMarkers.forEach(function (m) { self.map.removeLayer(m); });
        this.poiMarkers = [];

        var pois = JSON.parse(poisJson);
        pois.forEach(function (poi, idx) {
            var marker = L.marker([poi.latitude, poi.longitude], {
                icon: self._createPoiIcon(idx + 1)
            }).addTo(self.map)
                .bindPopup('<strong>' + (idx + 1) + '. ' + poi.poiName + '</strong><br/>' + (poi.description || ''));

            self.poiMarkers.push(marker);
        });
    },

    // Đặt chế độ click
    setClickMode: function (mode) {
        this.clickMode = mode;
    },

    // Dọn dẹp
    dispose: function () {
        if (this.map) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }
    }
};

// ===== HEATMAP (new) =====
window.heatmapInterop = {
    map: null,
    heatLayer: null,
    
    initHeatmap: function (elementId, dataPoints) {
        // Cleanup old map if exists
        if (this.map !== null) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }

        // Initialize map with Light (Positron) theme
        var tileLayer = L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
            subdomains: 'abcd',
            maxZoom: 20
        });

        // Tọa độ Vĩnh Khánh: 10.7615, 106.7022
        this.map = L.map(elementId).setView([10.7615, 106.7022], 16);
        tileLayer.addTo(this.map);

        // If no data points provided, use sample data
        var points = dataPoints;
        if (!points || points.length === 0) {
            points = [
                [10.7615, 106.7022, 1.0], [10.7616, 106.7023, 0.8], [10.7614, 106.7021, 0.9],
                [10.7612, 106.7020, 0.6], [10.7610, 106.7018, 0.5], [10.7617, 106.7025, 0.7],
                [10.7620, 106.7028, 0.6], [10.7618, 106.7024, 0.8], [10.7615, 106.7019, 0.5],
                [10.7613, 106.7022, 0.9], [10.7611, 106.7023, 0.4], [10.7616, 106.7020, 0.7]
            ];
        } else {
             // Parse JSON array if it's sent as string from Blazor
            if (typeof points === 'string') {
                points = JSON.parse(points);
            }
        }

        // Add heatLayer with orange color scheme
        this.heatLayer = L.heatLayer(points, {
            radius: 25,
            blur: 15,
            maxZoom: 17,
            gradient: {
                0.4: 'yellow',
                0.7: 'orange',
                1.0: '#FF7A00'  // Custom orange hex
            }
        }).addTo(this.map);

        var self = this;
        setTimeout(function() {
            if (self.map) self.map.invalidateSize();
        }, 100);
    },

    updateHeatmap: function(dataPoints) {
        if (!this.heatLayer) return;
        
        var points = dataPoints;
        if (typeof points === 'string') {
            points = JSON.parse(points);
        }
        
        // Cập nhật lại mảng toạ độ mới cho lớp điểm nhiệt
        this.heatLayer.setLatLngs(points);
    },

    dispose: function () {
        if (this.map) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }
    }
};
