using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Android.Gms.Location;
using System.Diagnostics;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;
using VinhKhanhTour.Utilities;

namespace VinhKhanhTour.Platforms.Android
{
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeLocation)]
    public class LocationService : Service
    {
        private const string ChannelId = "location_notification_channel";
        private const int NotificationId = 12345678;

        public override IBinder? OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            CreateNotificationChannel();
            var notification = CreateNotification();
            StartForeground(NotificationId, notification);

            _ = StartTracking();

            return StartCommandResult.Sticky;
        }

        private async Task StartTracking()
        {
            try
            {
                var request = new GeolocationListeningRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(2));
                
                Geolocation.Default.LocationChanged += OnLocationChanged;
                await Geolocation.Default.StartListeningForegroundAsync(request);
                
                System.Diagnostics.Debug.WriteLine("[LocationService] Started tracking using MAUI Geolocation");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LocationService] Error starting tracking: {ex.Message}");
            }
        }

        private async void OnLocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            var location = e.Location;
            System.Diagnostics.Debug.WriteLine($"[BackgroundLocation] {location.Latitude}, {location.Longitude}");

            // Update static property for easy access from pages
            BackgroundLocationTracker.CurrentLocation = location;
            
            // Trigger Geofence check
            await BackgroundLocationTracker.CheckGeofences(location);
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Vĩnh Khánh Tour Background Tracking", NotificationImportance.Low);
                var manager = (NotificationManager)GetSystemService(NotificationService)!;
                manager.CreateNotificationChannel(channel);
            }
        }

        private global::Android.App.Notification CreateNotification()
        {
            var intent = new Intent(this, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Immutable);

            return new NotificationCompat.Builder(this, ChannelId)
                .SetContentTitle("Vĩnh Khánh Food Tour")
                .SetContentText("Đang theo dõi vị trí để thuyết minh tự động...")
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent)
                .Build();
        }

        public override void OnDestroy()
        {
            Geolocation.Default.LocationChanged -= OnLocationChanged;
            Geolocation.Default.StopListeningForeground();
            base.OnDestroy();
        }
    }

    public static class BackgroundLocationTracker
    {
        public static Microsoft.Maui.Devices.Sensors.Location? CurrentLocation { get; set; }
        public static event EventHandler<Microsoft.Maui.Devices.Sensors.Location>? LocationChanged;

        public static async Task CheckGeofences(Microsoft.Maui.Devices.Sensors.Location userLocation)
        {
            LocationChanged?.Invoke(null, userLocation);

            var engine = IPlatformApplication.Current?.Services.GetService<NarrationEngine>();
            if (engine == null) return;

            var pois = Poi.GetSampleData();
            foreach (var poi in pois)
            {
                double distance = LocationHelper.CalculateDistanceInMeters(
                    userLocation.Latitude, userLocation.Longitude,
                    poi.Latitude, poi.Longitude);

                if (distance <= poi.Radius)
                {
                    await engine.PlayPoiNarrationAsync(poi);
                    break;
                }
            }
        }
    }
}
