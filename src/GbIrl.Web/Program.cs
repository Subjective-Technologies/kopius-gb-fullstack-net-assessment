using GbIrl.Infrastructure.Data;
using GbIrl.Infrastructure.Excel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var appDbConnectionString = AppContext.BaseDirectory.Contains("GbIrl.Spec.Tests", StringComparison.OrdinalIgnoreCase)
    ? $"Data Source={Path.Combine(Path.GetTempPath(), $"gbirl-tests-{Guid.NewGuid():N}.db")}"
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(appDbConnectionString));
builder.Services.AddScoped<ExcelImportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
    }
    catch (SqliteException ex) when (ex.SqliteErrorCode == 1 &&
                                    ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
    {
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program;
