using Microsoft.Data.Sqlite;
using SQLite;
using System.Diagnostics;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.Shared.Services;

namespace VinhKhanhTour.Shared.Data
{
    public class PoiRepository
    {
        private SQLiteAsyncConnection? _connection;
        private readonly IErrorHandler _errorHandler;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private bool _hasInitialized = false;

        public PoiRepository(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        private async Task InitAsync()
        {
            if (_hasInitialized && _connection is not null) return;

            await _initLock.WaitAsync();
            try
            {
                if (_hasInitialized && _connection is not null) return;

                var dbPath = Constants.DatabasePath.Replace("Data Source=", "");

                _connection = new SQLiteAsyncConnection(dbPath);

                // One-time fix (v2): drop Poi table to clear encoding-corrupted data
                var flagFile = dbPath + ".encoding_fix_v2";
                if (!File.Exists(flagFile))
                {
                    try
                    {
                        await _connection.DropTableAsync<Poi>();
                        File.WriteAllText(flagFile, "done");
                        Debug.WriteLine("[DB Migration] Dropped Poi table to fix encoding issues.");
                    }
                    catch { /* Table might not exist yet, that's fine */ }
                }
                
                var result = await _connection.CreateTableAsync<Poi>();
                await _connection.CreateTableAsync<Tour>();
                await _connection.CreateTableAsync<TourStop>();
                await _connection.CreateTableAsync<UsageHistory>();
                await _connection.CreateTableAsync<UserRoute>();

                if (result == CreateTableResult.Created)
                {
                    // Seed sample data for fresh table
                    var sampleData = Poi.GetSampleData();
                    await _connection.InsertAllAsync(sampleData);
                }

                _hasInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<List<Poi>> GetAllPoisAsync()
        {
            try
            {
                await InitAsync();
                return await _connection!.Table<Poi>().ToListAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
                Debug.WriteLine($"Failed to retrieve POIs: {ex.Message}");
            }

            return new List<Poi>();
        }

        public async Task<int> SavePoiAsync(Poi poi)
        {
            try
            {
                await InitAsync();
                if (poi.Id != 0)
                {
                    return await _connection!.UpdateAsync(poi);
                }
                else
                {
                    return await _connection!.InsertAsync(poi);
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
                Debug.WriteLine($"Failed to save POI: {ex.Message}");
            }

            return 0;
        }

        public async Task<int> DeletePoiAsync(Poi poi)
        {
            try
            {
                await InitAsync();
                return await _connection!.DeleteAsync(poi);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
                Debug.WriteLine($"Failed to delete POI: {ex.Message}");
            }

            return 0;
        }

        public async Task<Poi?> GetPoiAsync(int id)
        {
            try
            {
                await InitAsync();
                return await _connection!.Table<Poi>().Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return null;
        }

        public async Task<int> AddPoiAsync(Poi poi)
        {
            try
            {
                await InitAsync();
                return await _connection!.InsertAsync(poi);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return 0;
        }

        public async Task<int> UpdatePoiAsync(Poi poi)
        {
            try
            {
                await InitAsync();
                return await _connection!.UpdateAsync(poi);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return 0;
        }

        // --- TOUR CRUD METHODS ---
        public async Task<List<Tour>> GetAllToursAsync()
        {
            try
            {
                await InitAsync();
                return await _connection!.Table<Tour>().ToListAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return new List<Tour>();
        }

        public async Task<int> SaveTourAsync(Tour tour)
        {
            try
            {
                await InitAsync();
                if (tour.Id != 0)
                {
                    return await _connection!.UpdateAsync(tour);
                }
                else
                {
                    return await _connection!.InsertAsync(tour);
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return 0;
        }

        public async Task<int> DeleteTourAsync(Tour tour)
        {
            try
            {
                await InitAsync();
                return await _connection!.DeleteAsync(tour);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return 0;
        }

        // --- ANALYTICS / HISTORY METHODS ---
        public async Task<List<UsageHistory>> GetUsageHistoryAsync()
        {
            try
            {
                await InitAsync();
                return await _connection!.Table<UsageHistory>().OrderByDescending(h => h.Timestamp).ToListAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return new List<UsageHistory>();
        }

        public async Task<int> RecordUsageAsync(UsageHistory history)
        {
            try
            {
                await InitAsync();
                return await _connection!.InsertAsync(history);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            return 0;
        }
    }
}
