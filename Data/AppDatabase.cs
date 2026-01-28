using SQLite;
using JournalSquare.Models;

namespace JournalSquare.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public AppDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        _database.CreateTableAsync<UserSettings>().Wait();
        _database.CreateTableAsync<JournalEntry>().Wait();
        _database.CreateTableAsync<Tag>().Wait();
        _database.CreateTableAsync<EntryTag>().Wait();
    }

    public SQLiteAsyncConnection Database => _database;
}