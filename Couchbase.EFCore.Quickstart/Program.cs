using System.Text;
using Couchbase;
using Couchbase.EFCore.Quickstart.Data;
using Couchbase.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var description = new StringBuilder()
        .AppendLine("A quickstart API using C# and ASP.NET with Couchbase and travel-sample data.\n\n")
        .AppendLine("This API documentation uses Swagger to provide an interactive UI for exploring and testing endpoints.\n\n")
        .AppendLine("### Features of Swagger UI:\n\n")
        .AppendLine("- Click on an endpoint to expand details and view parameters.\n")
        .AppendLine("- Use the \"Try it out\" button to send requests directly from the browser.\n")
        .AppendLine("- View response models and data structures.\n\n")
        .AppendLine("### API Details:\n\n")
        .AppendLine("For more details, visit the Couchbase Developer Portal: https://developer.couchbase.com/tutorial-quickstart-csharp-aspnet\n\n")
        .ToString();

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Quickstart API with Couchbase & EF Core",
        Description = description
    });

    options.EnableAnnotations();
});

builder.Services.AddDbContext<TravelDbContext>(options =>
{
    var clusterOptions = new ClusterOptions()
        .WithCredentials("", "")
        .WithConnectionString("")
        .WithLogging(
            LoggerFactory.Create(
                builder =>
                {
                    builder.AddFilter(level => level >= LogLevel.Debug);
                }));
    options
        .UseCouchbase(clusterOptions,
            couchbaseDbContextOptions =>
            {
                couchbaseDbContextOptions.Bucket = "travel-sample";
                couchbaseDbContextOptions.Scope = "inventory";
            });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quickstart API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();