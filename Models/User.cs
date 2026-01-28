using SQLite;

namespace JournalSquare.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public string Name { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;
}