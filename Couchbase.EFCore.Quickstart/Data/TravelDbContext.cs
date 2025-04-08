using Couchbase.EFCore.Quickstart.Models;
using Couchbase.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Route = Couchbase.EFCore.Quickstart.Models.Route;

namespace Couchbase.EFCore.Quickstart.Data;

public class TravelDbContext: DbContext
{
    
    public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
    {
    }
    
    public DbSet<Route> Routes { get; set; }
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Airport> Airports { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Route>().ToCouchbaseCollection(this, "route");
        modelBuilder.Entity<Schedule>().HasNoKey();
        modelBuilder.Ignore<Schedule>();

        modelBuilder.Entity<Airline>().ToCouchbaseCollection(this, "airline");
        
        modelBuilder.Entity<Airport>().ToCouchbaseCollection(this, "airport");
        modelBuilder.Entity<Geo>().HasNoKey();
        modelBuilder.Ignore<Geo>();
    }
}