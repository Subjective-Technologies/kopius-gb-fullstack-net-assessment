using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GbIrl.Spec.Tests.Support;

public class GbIrlWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        // In-memory SQLite / test DB wiring goes here when persistence is implemented.
    }
}
