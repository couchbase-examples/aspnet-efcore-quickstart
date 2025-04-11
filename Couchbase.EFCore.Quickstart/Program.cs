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
        .AppendLine("A quickstart API using C# and ASP.NET with Couchbase EFCore provider, and travel-sample data.\n\n")
        .AppendLine("This API provides a visual and interactive documentation experience using Swagger. You can explore and interact with the available endpoints directly through the browser interface. The documentation offers a clear view of the API's endpoints, HTTP methods, request parameters, and response formats.\n\n")
        .AppendLine("Click on any endpoint to expand it and view comprehensive details. You'll see the endpoint's description, expected request parameters, and possible response status codes.\n\n")
        .AppendLine("Trying Out the API\n\n")
        .AppendLine("Swagger UI allows you to try out API operations easily by clicking the \"Try it out\" button.\n\n")
        .AppendLine("- Parameters: For endpoints that require input, Swagger UI provides fields for you to enter data such as path parameters, query strings, headers, or the body content for POST/PUT requests.\n\n")
        .AppendLine("- Execution: After providing the necessary parameters, click \"Execute\" to make a live API call. The response will be displayed in real-time, including the response code, headers, and body content.\n\n")
        .AppendLine("Models\n\n")
        .AppendLine("Swagger documents the request and response structures using models. These models illustrate the expected JSON schema, making it easier to understand the data format you need to send and what you can expect in return.\n\n")
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