using DockerTrips.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DockerTrips.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Trip> Trips => Set<Trip>();
}