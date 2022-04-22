using Microsoft.EntityFrameworkCore;
using SqlBundle.Models;
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<SqlContext>(opt =>
{
    var config = builder.Configuration;

    var server   = config["DBserver"] ?? "178.154.204.217";
    var port     = config["DBport"] ?? "5432";
    var dbName   = config["DBname"] ?? "Bundle";
    var user     = config["DBuser"] ?? "postgres";
    var password = config["DBPassword"] ?? "root";

    var conectionString = $"Host={server};Port={port};Database={dbName};Username={user};Password={password}";
    opt.UseNpgsql(conectionString);
});

builder.Services.AddTransient<BundleRepo>();
builder.PopulateDB();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
