using Microsoft.Data.Sqlite;
using System.IO;

namespace WaterTracker
{
    public class WaterRepository
    {
        private readonly string _dbPath;

        public WaterRepository()
        {
            var baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WaterTracker");

            Directory.CreateDirectory(baseDir);
            _dbPath = Path.Combine(baseDir, "watertracker.db");
        }

        private SqliteConnection CreateConnection()
            => new SqliteConnection($"Data Source={_dbPath}");

        public async Task InitializeAsync()
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS WaterEntry (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Timestamp TEXT NOT NULL,
                AmountMl INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Settings (
                Key TEXT PRIMARY KEY,
                Value TEXT NOT NULL
            );
            ";
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task AddEntryAsync(DateTime timestamp, int amountMl)
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO WaterEntry (Timestamp, AmountMl) VALUES ($ts, $ml);";
            cmd.Parameters.AddWithValue("$ts", timestamp.ToString("o")); // ISO format
            cmd.Parameters.AddWithValue("$ml", amountMl);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> GetTotalForDateAsync(DateTime date)
        {
            var from = date.Date;
            var to = from.AddDays(1);

            using var con = CreateConnection();
            await con.OpenAsync();

            var cmd = con.CreateCommand();
            cmd.CommandText =
                @"SELECT COALESCE(SUM(AmountMl), 0)
                  FROM WaterEntry
                  WHERE Timestamp >= $from AND Timestamp < $to;";

            cmd.Parameters.AddWithValue("$from", from.ToString("o"));
            cmd.Parameters.AddWithValue("$to", to.ToString("o"));

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<List<WaterEntry>> GetEntriesForDateAsync(DateTime date)
        {
            var from = date.Date;
            var to = from.AddDays(1);

            using var con = CreateConnection();
            await con.OpenAsync();

            var cmd = con.CreateCommand();
            cmd.CommandText =
                @"SELECT Id, Timestamp, AmountMl
                  FROM WaterEntry
                  WHERE Timestamp >= $from AND Timestamp < $to;";

            cmd.Parameters.AddWithValue("$from", from.ToString("o"));
            cmd.Parameters.AddWithValue("$to", to.ToString("o"));

            var list = new List<WaterEntry>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new WaterEntry
                {
                    Id = reader.GetInt32(0),
                    Timestamp = DateTime.Parse(reader.GetString(1)),
                    AmountMl = reader.GetInt32(2)
                });
            }
            return list;
        }

        public async Task<int?> GetDailyGoalAsync()
        {
            return await GetSettingIntAsync("DailyGoalMl");
        }

        public async Task SetDailyGoalAsync(int ml)
        {
            await SetSettingAsync("DailyGoalMl", ml.ToString());
        }

        private async Task<int?> GetSettingIntAsync(string key)
        {
            var s = await GetSettingAsync(key);
            if (s is null) return null;
            return int.TryParse(s, out var v) ? v : null;
        }

        private async Task<string?> GetSettingAsync(string key)
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT Value FROM Settings WHERE Key = $k;";
            cmd.Parameters.AddWithValue("$k", key);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }

        private async Task SetSettingAsync(string key, string value)
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var cmd = con.CreateCommand();
            cmd.CommandText =
                @"INSERT INTO Settings(Key, Value) VALUES($k, $v)
                  ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;";
            cmd.Parameters.AddWithValue("$k", key);
            cmd.Parameters.AddWithValue("$v", value);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public class WaterEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int AmountMl { get; set; }
    }
}
