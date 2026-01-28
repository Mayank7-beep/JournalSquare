using JournalSquare.Data;
using JournalSquare.Models;
using SQLite;

namespace JournalSquare.Services.Analytics;

public class AnalyticsService
{
    private readonly SQLiteAsyncConnection _db;

    public AnalyticsService(AppDatabase database)
    {
        _db = database.Database;
    }

    // =========================
    // BASIC ANALYTICS
    // =========================

    public async Task<int> GetTotalEntriesAsync()
    {
        return await _db.Table<JournalEntry>().CountAsync();
    }

    public async Task<int> GetAverageWordCountAsync()
    {
        var entries = await _db.Table<JournalEntry>().ToListAsync();

        if (entries.Count == 0)
            return 0;

        return (int)entries.Average(e => e.WordCount);
    }

    public async Task<string> GetMostCommonMoodAsync()
    {
        var entries = await _db.Table<JournalEntry>().ToListAsync();

        return entries
            .Where(e => !string.IsNullOrWhiteSpace(e.PrimaryMood))
            .GroupBy(e => e.PrimaryMood)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "—";
    }

    // =========================
    // STREAK / MISSED DAYS
    // =========================

    public async Task<int> GetMissedDaysAsync()
    {
        var entries = await _db.Table<JournalEntry>().ToListAsync();

        if (entries.Count == 0)
            return 0;

        var dates = entries
            .Select(e => e.EntryDate.Date)
            .Distinct()
            .ToList();

        var firstDate = dates.Min();
        var today = DateTime.Today;

        var totalDays = (today - firstDate).Days + 1;
        return totalDays - dates.Count;
    }

    // =========================
    // TAG ANALYTICS
    // =========================

    public async Task<string> GetMostUsedTagAsync()
    {
        var entries = await _db.Table<JournalEntry>().ToListAsync();

        var tags = entries
            .Where(e => !string.IsNullOrWhiteSpace(e.Tags))
            .SelectMany(e =>
                e.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(t => t.Trim());

        return tags
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "—";
    }

    public async Task<Dictionary<string, int>> GetTagBreakdownAsync()
    {
        var entries = await _db.Table<JournalEntry>().ToListAsync();

        return entries
            .Where(e => !string.IsNullOrWhiteSpace(e.Tags))
            .SelectMany(e =>
                e.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(t => t.Trim())
            .GroupBy(t => t)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
