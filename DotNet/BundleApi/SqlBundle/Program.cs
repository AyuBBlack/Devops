using Microsoft.EntityFrameworkCore;
using Prometheus;
using SqlBundle.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<SqlContext>(opt =>
{
    var config = builder.Configuration;

    var server = config["DBserver"] ?? "51.250.69.110";
    var port = config["DBport"] ?? "5432";
    var dbName = config["DBname"] ?? "Bundle";
    var user = config["DBuser"] ?? "postgres";
    var password = config["DBPassword"] ?? "root";

    var conectionString = $"Host={server};Port={port};Database={dbName};Username={user};Password={password}";
    opt.UseNpgsql(conectionString);
});

builder.Services.AddTransient<ChangeDB>();
builder.PopulateDB();

var app = builder.Build();

app.UseMetricServer();

app.UseHttpMetrics();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();