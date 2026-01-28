using JournalSquare.Data;
using JournalSquare.Models;
using SQLite;

namespace JournalSquare.Services;

public class StreakService
{
    private readonly SQLiteAsyncConnection _db;

    public StreakService(AppDatabase database)
    {
        _db = database.Database;
    }

    // ✅ Current streak (consecutive days up to today)
    public async Task<int> GetCurrentStreakAsync()
    {
        var dates = await GetAllEntryDatesAsync();

        int streak = 0;
        DateTime day = DateTime.Today;

        while (dates.Contains(day))
        {
            streak++;
            day = day.AddDays(-1);
        }

        return streak;
    }

    // ✅ Longest streak ever
    public async Task<int> GetLongestStreakAsync()
    {
        var dates = await GetAllEntryDatesAsync();
        if (!dates.Any())
            return 0;

        int longest = 1;
        int current = 1;

        var ordered = dates.OrderBy(d => d).ToList();

        for (int i = 1; i < ordered.Count; i++)
        {
            if (ordered[i] == ordered[i - 1].AddDays(1))
            {
                current++;
                longest = Math.Max(longest, current);
            }
            else
            {
                current = 1;
            }
        }

        return longest;
    }

    // ✅ Missed days between first entry and today
    public async Task<List<DateTime>> GetMissedDaysAsync()
    {
        var dates = await GetAllEntryDatesAsync();
        var missed = new List<DateTime>();

        if (!dates.Any())
            return missed;

        DateTime start = dates.Min();
        DateTime end = DateTime.Today;

        for (var day = start; day <= end; day = day.AddDays(1))
        {
            if (!dates.Contains(day))
                missed.Add(day);
        }

        return missed;
    }

    // 🔹 Helper: fetch all journal dates (FIXED)
    private async Task<HashSet<DateTime>> GetAllEntryDatesAsync()
    {
        // Fetch full entries FIRST (SQLite limitation)
        var entries = await _db.Table<JournalEntry>().ToListAsync();

        // Then project in memory
        return entries
            .Select(e => e.EntryDate.Date)
            .ToHashSet();
    }
}
