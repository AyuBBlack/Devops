using Microsoft.EntityFrameworkCore;
using SqlBundle.Models;
using SqlBundle.Logging;
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<SqlContext>(opt =>
{
    var config = builder.Configuration;

    var server   = config["DBserver"] ?? "localhost";
    var port     = config["DBport"] ?? "5432";
    var dbName   = config["DBname"] ?? "Bundle";
    var user     = config["DBuser"] ?? "postgres";
    var password = config["DBPassword"] ?? "1234";

    var conectionString = $"Host={server};Port={port};Database={dbName};Username={user};Password={password}";
    opt.UseNpgsql(conectionString);
});

builder.Services.AddTransient<BundleRepo>();

builder.Logging.ClearProviders()
    .AddDbLogger(configure => { });

builder.PopulateDB();

/*builder.Logging.ClearProviders()
    .AddConsole()
    .AddDebug();*/

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
