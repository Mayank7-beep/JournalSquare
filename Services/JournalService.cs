using JournalSquare.Data;
using JournalSquare.Models;
using SQLite;

namespace JournalSquare.Services;

public class JournalService
{
    private readonly SQLiteAsyncConnection _db;

    public JournalService(AppDatabase database)
    {
        _db = database.Database;
    }

    // =======================
    // CREATE / UPDATE (ONE PER DAY)
    // =======================
    public async Task SaveEntryAsync(JournalEntry entry)
    {
        var entryDate = entry.EntryDate.Date;

        var existingEntry = await _db.Table<JournalEntry>()
            .Where(e => e.EntryDate == entryDate)
            .FirstOrDefaultAsync();

        entry.EntryDate = entryDate;
        entry.WordCount = CountWords(entry.Content);

        if (existingEntry == null)
        {
            entry.CreatedAt = DateTime.Now;
            entry.UpdatedAt = DateTime.Now;
            await _db.InsertAsync(entry);
        }
        else
        {
            entry.Id = existingEntry.Id;
            entry.CreatedAt = existingEntry.CreatedAt;
            entry.UpdatedAt = DateTime.Now;
            await _db.UpdateAsync(entry);
        }
    }

    // =======================
    // DELETE
    // =======================
    public async Task DeleteEntryAsync(int entryId)
    {
        await _db.DeleteAsync<JournalEntry>(entryId);
    }

    // =======================
    // SINGLE DAY FETCH
    // =======================
    public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
    {
        var targetDate = date.Date;

        return await _db.Table<JournalEntry>()
            .Where(e => e.EntryDate == targetDate)
            .FirstOrDefaultAsync();
    }

    // =======================
    // ALL ENTRIES (CALENDAR)
    // =======================
    public async Task<List<JournalEntry>> GetAllEntriesAsync()
    {
        return await _db.Table<JournalEntry>()
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    // =======================
    // PAGINATION
    // =======================
    public async Task<List<JournalEntry>> GetPagedEntriesAsync(int page, int pageSize)
    {
        return await _db.Table<JournalEntry>()
            .OrderByDescending(e => e.EntryDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // =======================
    // 🔍 SEARCH (TITLE, CONTENT, MOOD, TAGS)
    // =======================
    public async Task<List<JournalEntry>> SearchAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        string? primaryMood)
    {
        var entries = await _db.Table<JournalEntry>().ToListAsync();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.ToLower();

            entries = entries.Where(e =>
                (!string.IsNullOrEmpty(e.Title) && e.Title.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(e.Content) && e.Content.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(e.PrimaryMood) && e.PrimaryMood.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(e.SecondaryMoods) && e.SecondaryMoods.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(e.Tags) && e.Tags.ToLower().Contains(keyword))
            ).ToList();
        }

        if (startDate.HasValue)
            entries = entries.Where(e => e.EntryDate >= startDate.Value.Date).ToList();

        if (endDate.HasValue)
            entries = entries.Where(e => e.EntryDate <= endDate.Value.Date).ToList();

        if (!string.IsNullOrWhiteSpace(primaryMood))
            entries = entries.Where(e =>
                e.PrimaryMood?.Equals(primaryMood, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();

        return entries
            .OrderByDescending(e => e.EntryDate)
            .ToList();
    }

    // =======================
    // DASHBOARD HELPERS
    // =======================
    public async Task<int> GetTotalCountAsync()
    {
        return await _db.Table<JournalEntry>().CountAsync();
    }

    public async Task<List<JournalEntry>> GetRecentEntriesAsync(int count)
    {
        return await _db.Table<JournalEntry>()
            .OrderByDescending(e => e.EntryDate)
            .Take(count)
            .ToListAsync();
    }

    // =======================
    // WORD COUNT
    // =======================
    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(
            new[] { ' ', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries
        ).Length;
    }
}
