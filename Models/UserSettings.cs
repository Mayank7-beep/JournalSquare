using SQLite;

namespace JournalSquare.Models;

public class UserSettings
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string PinHash { get; set; } = string.Empty;

    public string Theme { get; set; } = "Light";
}