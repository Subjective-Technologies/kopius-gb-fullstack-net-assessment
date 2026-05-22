using System.ComponentModel.DataAnnotations;
using GbIrl.Core.Entities;
using GbIrl.Core.Enums;
using GbIrl.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GbIrl.Web.Pages.Items;

[IgnoreAntiforgeryToken]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    public IrlItem? Item { get; private set; }

    [BindProperty]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [BindProperty]
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal UnitCost { get; set; }

    [BindProperty]
    public ItemCondition Condition { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Item = await _db.Items.FindAsync(id);
        if (Item is null)
            return NotFound();

        Quantity = Item.Quantity;
        UnitCost = Item.UnitCost;
        Condition = Item.Condition;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Item = await _db.Items.FindAsync(id);
        if (Item is null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        Item.Quantity = Quantity;
        Item.UnitCost = UnitCost;
        Item.Condition = Condition;
        await _db.SaveChangesAsync();

        return RedirectToPage("/Items");
    }
}
