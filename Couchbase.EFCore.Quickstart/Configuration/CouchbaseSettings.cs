using System.ComponentModel.DataAnnotations;

namespace Couchbase.EFCore.Quickstart.Configuration;

/// <summary>
/// Cluster connection settings, bound from the "Couchbase" configuration section.
/// Supply them through appsettings, user-secrets, or environment variables
/// (for example <c>Couchbase__ConnectionString</c>).
/// </summary>
public class CouchbaseSettings
{
    public const string SectionName = "Couchbase";

    /// <summary>The travel-sample bucket and inventory scope the models and SQL++ queries use.</summary>
    public const string BucketName = "travel-sample";
    public const string ScopeName = "inventory";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
