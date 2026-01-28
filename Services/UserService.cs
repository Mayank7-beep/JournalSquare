using JournalSquare.Data;
using JournalSquare.Models;
using SQLite;


namespace JournalSquare.Services;

public class UserService
{
    private readonly SQLiteAsyncConnection _db;

    public UserService(AppDatabase database)
    {
        _db = database.Database;
    }

    public async Task<bool> RegisterAsync(string name, string pin)
    {
        if (string.IsNullOrWhiteSpace(name) || pin.Length < 4)
            return false;

        var existing = await _db.Table<User>()
            .Where(u => u.Name == name)
            .FirstOrDefaultAsync();

        if (existing != null)
            return false;

        await _db.InsertAsync(new User
        {
            Name = name,
            Pin = pin
        });

        return true;
    }

    public async Task<bool> LoginAsync(string name, string pin)
    {
        var user = await _db.Table<User>()
            .Where(u => u.Name == name && u.Pin == pin)
            .FirstOrDefaultAsync();

        return user != null;
    }
}