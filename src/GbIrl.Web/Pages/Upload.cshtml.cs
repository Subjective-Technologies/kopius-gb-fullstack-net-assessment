using GbIrl.Infrastructure.Data;
using GbIrl.Infrastructure.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GbIrl.Web.Pages;

[IgnoreAntiforgeryToken]
public class UploadModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly ExcelImportService _importService;

    public UploadModel(AppDbContext db, ExcelImportService importService)
    {
        _db = db;
        _importService = importService;
    }

    public bool HasResult { get; private set; }
    public int LoadedCount { get; private set; }
    public int RejectedCount { get; private set; }
    public IReadOnlyList<ImportRowError> RejectedRows { get; private set; } = Array.Empty<ImportRowError>();
    public string? Message { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            Message = "Choose an .xlsx file to upload.";
            return Page();
        }

        await using var stream = file.OpenReadStream();
        var result = _importService.Import(stream);

        await _db.Items.ExecuteDeleteAsync();
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name = 'Items'");
        _db.Items.AddRange(result.LoadedItems);
        await _db.SaveChangesAsync();

        HasResult = true;
        LoadedCount = result.LoadedCount;
        RejectedCount = result.RejectedCount;
        RejectedRows = result.RejectedRows;

        return Page();
    }
}
