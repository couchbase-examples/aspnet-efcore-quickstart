# aspnet-efcore-quickstart
A sample ASP.NET app using Couchbase and EF Core to demonstrate CRUD operations and basic queries with sample data models.

## Configuration

The app reads its cluster connection from the `Couchbase` configuration section and refuses to start if any value is missing:

| Setting | Environment variable |
| --- | --- |
| `Couchbase:ConnectionString` | `Couchbase__ConnectionString` |
| `Couchbase:Username` | `Couchbase__Username` |
| `Couchbase:Password` | `Couchbase__Password` |

`appsettings.Development.json` ships local defaults (`couchbase://localhost`, `Administrator` / `password`). For Capella or any shared cluster, use user-secrets or environment variables instead of editing the file:

```sh
cd Couchbase.EFCore.Quickstart
dotnet user-secrets init
dotnet user-secrets set "Couchbase:ConnectionString" "couchbases://<your-endpoint>.cloud.couchbase.com"
dotnet user-secrets set "Couchbase:Username" "<username>"
dotnet user-secrets set "Couchbase:Password" "<password>"
dotnet run
```

The app uses the `travel-sample` bucket and `inventory` scope.

## Tests

```sh
dotnet test
```

The tests check the startup wiring (settings binding, fail-fast validation, DbContext model) and don't need a running cluster.
