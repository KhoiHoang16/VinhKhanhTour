namespace VinhKhanhTour.Data
{
    public static class Constants
    {
        public const string DatabaseFilename = "GastronomyTour.db3";

        public static string DatabasePath =>
            $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";
    }
}