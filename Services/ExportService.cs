using JournalSquare.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace JournalSquare.Services;

public class ExportService
{
    public async Task ExportToPdfAsync(
        List<JournalEntry> entries,
        string filePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Column(col =>
                    {
                        col.Item().Text("JournalSquare – Exported Journals")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();

                        col.Item().PaddingVertical(10).LineHorizontal(1);

                        foreach (var entry in entries)
                        {
                            col.Item().PaddingBottom(10).Column(entryCol =>
                            {
                                entryCol.Item().Text(entry.Title).Bold();
                                entryCol.Item().Text($"Date: {entry.EntryDate:dd MMM yyyy}");
                                entryCol.Item().Text($"Mood: {entry.PrimaryMood}");
                                entryCol.Item().Text($"Tags: {entry.Tags}");
                                entryCol.Item().PaddingTop(5).Text(entry.Content);
                            });

                            col.Item().LineHorizontal(0.5f);
                        }
                    });
                });
            })
            .GeneratePdf(filePath);

        await Task.CompletedTask;
    }
}