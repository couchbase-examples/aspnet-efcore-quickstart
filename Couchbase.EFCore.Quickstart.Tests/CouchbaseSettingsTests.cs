using Couchbase.EFCore.Quickstart.Configuration;
using Couchbase.EFCore.Quickstart.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Couchbase.EFCore.Quickstart.Tests;

/// <summary>
/// Startup wiring checks that run without a Couchbase cluster: nothing here opens a connection.
/// </summary>
public class CouchbaseSettingsTests
{
    private static WebApplicationFactory<Program> CreateFactory(IDictionary<string, string?> settings) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            foreach (var (key, value) in settings)
            {
                builder.UseSetting(key, value);
            }
        });

    [Fact]
    public void Startup_fails_fast_when_connection_settings_are_missing()
    {
        using var factory = CreateFactory(new Dictionary<string, string?>
        {
            ["Couchbase:ConnectionString"] = "",
            ["Couchbase:Username"] = "",
            ["Couchbase:Password"] = "",
        });

        var ex = Assert.Throws<OptionsValidationException>(() => factory.Services);

        Assert.Contains(ex.Failures, f => f.Contains(nameof(CouchbaseSettings.ConnectionString)));
        Assert.Contains(ex.Failures, f => f.Contains(nameof(CouchbaseSettings.Username)));
        Assert.Contains(ex.Failures, f => f.Contains(nameof(CouchbaseSettings.Password)));
    }

    [Fact]
    public void Connection_settings_bind_from_configuration()
    {
        using var factory = CreateFactory(new Dictionary<string, string?>
        {
            ["Couchbase:ConnectionString"] = "couchbases://cb.example.cloud.couchbase.com",
            ["Couchbase:Username"] = "app-user",
            ["Couchbase:Password"] = "app-password",
        });

        var settings = factory.Services.GetRequiredService<IOptions<CouchbaseSettings>>().Value;

        Assert.Equal("couchbases://cb.example.cloud.couchbase.com", settings.ConnectionString);
        Assert.Equal("app-user", settings.Username);
        Assert.Equal("app-password", settings.Password);
    }

    [Fact]
    public void DbContext_builds_its_model_from_configured_settings()
    {
        using var factory = CreateFactory(new Dictionary<string, string?>
        {
            ["Couchbase:ConnectionString"] = "couchbase://localhost",
            ["Couchbase:Username"] = "app-user",
            ["Couchbase:Password"] = "app-password",
        });

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TravelDbContext>();

        Assert.NotNull(context.Model.FindEntityType(typeof(Models.Airline)));
        Assert.NotNull(context.Model.FindEntityType(typeof(Models.Route)));
    }
}
