using GbIrl.Core.Entities;
using GbIrl.Core.Enums;
using GbIrl.Core.Services;
using GbIrl.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GbIrl.Web.Pages;

public class ItemsModel : PageModel
{
    private readonly AppDbContext _db;

    public ItemsModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public ItemCondition? Condition { get; set; }

    public IReadOnlyList<IrlItem> Items { get; private set; } = Array.Empty<IrlItem>();
    public IReadOnlyList<string> Categories { get; private set; } = Array.Empty<string>();
    public decimal TotalOlv { get; private set; }
    public decimal WeightedAverageUnitCost { get; private set; }
    public decimal MedianUnitCost { get; private set; }
    public decimal StandardDeviationUnitCost { get; private set; }

    public async Task OnGetAsync()
    {
        Categories = await _db.Items
            .Select(i => i.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        Items = await IrlItemFilters.Apply(_db.Items.AsNoTracking(), Category, Condition)
            .OrderBy(i => i.Id)
            .ToListAsync();

        TotalOlv = IrlStatisticsService.TotalOlv(Items);
        WeightedAverageUnitCost = IrlStatisticsService.WeightedAverageUnitCost(Items);
        MedianUnitCost = IrlStatisticsService.MedianUnitCost(Items);
        StandardDeviationUnitCost = IrlStatisticsService.StandardDeviationUnitCost(Items);
    }
}
