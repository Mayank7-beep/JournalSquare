using SQLite;

namespace JournalSquare.Models;

public class JournalEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // 📅 One journal per day (used for streaks & uniqueness)
    [Indexed(Unique = true)]
    public DateTime EntryDate { get; set; }

    // 📝 Core content
    public string Title { get; set; } = string.Empty;

    // Markdown content
    public string Content { get; set; } = string.Empty;

    // ⏱ System-generated timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // 😊 Mood tracking
    public string PrimaryMood { get; set; } = string.Empty;

    // Secondary moods (comma-separated)
    public string SecondaryMoods { get; set; } = string.Empty;

    // 🏷 Tags (comma-separated, predefined + custom)
    public string Tags { get; set; } = string.Empty;

    // 📊 Analytics
    public int WordCount { get; set; }
}