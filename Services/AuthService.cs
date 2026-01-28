using JournalSquare.Data;
using JournalSquare.Models;
using SQLite;
using System.Security.Cryptography;
using System.Text;

namespace JournalSquare.Services;

public class AuthService
{
    private readonly SQLiteAsyncConnection _db;

    public AuthService(AppDatabase database)
    {
        _db = database.Database;
    }

    // ============================
    // SIGN UP / FIRST-TIME SETUP
    // ============================
    public async Task SetUserAsync(string userName, string pin)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(pin))
            return;

        userName = userName.Trim().ToLower();
        var hash = HashPin(pin);

        var settings = await _db.Table<UserSettings>().FirstOrDefaultAsync();

        if (settings == null)
        {
            await _db.InsertAsync(new UserSettings
            {
                UserName = userName,
                PinHash = hash,
                Theme = "Light"
            });
        }
        else
        {
            // Overwrite existing user (safe for single-user app)
            settings.UserName = userName;
            settings.PinHash = hash;
            await _db.UpdateAsync(settings);
        }
    }

    // ============================
    // LOGIN VALIDATION
    // ============================
    public async Task<bool> ValidateLoginAsync(string userName, string pin)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(pin))
            return false;

        userName = userName.Trim().ToLower();

        var settings = await _db.Table<UserSettings>().FirstOrDefaultAsync();
        if (settings == null)
            return false;

        return settings.UserName == userName
               && settings.PinHash == HashPin(pin);
    }

    // ============================
    // CHANGE PIN (SETTINGS PAGE)
    // ============================
    public async Task<bool> ChangePinAsync(string userName, string oldPin, string newPin)
    {
        if (string.IsNullOrWhiteSpace(userName)
            || string.IsNullOrWhiteSpace(oldPin)
            || string.IsNullOrWhiteSpace(newPin))
            return false;

        userName = userName.Trim().ToLower();

        var settings = await _db.Table<UserSettings>().FirstOrDefaultAsync();
        if (settings == null)
            return false;

        if (settings.UserName != userName)
            return false;

        if (settings.PinHash != HashPin(oldPin))
            return false;

        if (newPin.Length < 4)
            return false;

        settings.PinHash = HashPin(newPin);
        await _db.UpdateAsync(settings);

        return true;
    }

    // ============================
    // GET LOGGED-IN USER
    // ============================
    public async Task<string?> GetUserNameAsync()
    {
        var settings = await _db.Table<UserSettings>().FirstOrDefaultAsync();
        return settings?.UserName;
    }

    // ============================
    // HASHING (SHA-256)
    // ============================
    private string HashPin(string pin)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(pin);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}
