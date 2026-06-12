using DockerTrips.Api.Data;
using DockerTrips.Api.Models;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var server = Environment.GetEnvironmentVariable("DB_SERVER");
var database = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString =
    $"Server={server};Database={database};User Id={user};Password={password};TrustServerCertificate=True";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
var app = builder.Build();




app.MapPost("/trips", async (Trip trip, AppDbContext db) =>
{
    db.Trips.Add(trip);

    await db.SaveChangesAsync();

    return Results.Created($"/trips/{trip.Id}", trip);
});
app.MapGet("/trips", async (AppDbContext db) =>
{
    return await db.Trips.ToListAsync();
});

app.Run();