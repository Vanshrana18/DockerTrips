using DockerTrips.Api.Data;
using DockerTrips.Api.Models;
using Prometheus;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var server = Environment.GetEnvironmentVariable("DB_SERVER");
var database = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
//var connectionString=builder.Configuration.GetConnectionString("DefaultConnection");

var connectionString =
    $"Server={server};Database={database};User Id={user};Password={password};TrustServerCertificate=True";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
var meter = new Meter("DockerTrips");

var tripCreationCounter = Metrics.CreateCounter(
    "dockertrips_trip_creations_total",
    "Total number of trips created");
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}



app.MapPost("/trips", async (Trip trip, AppDbContext db) =>
{
    try
    {
        db.Trips.Add(trip);

        await db.SaveChangesAsync();

        tripCreationCounter.Inc();

        return Results.Created($"/trips/{trip.Id}", trip);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.ToString());
    }
});
app.MapGet("/trips", async (AppDbContext db) =>
{
    return await db.Trips.ToListAsync();
});
app.MapGet("/test", () =>
{
    return "Test endpoint is working!";
});
app.UseHttpMetrics();
app.MapMetrics();
app.Run();