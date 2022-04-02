using Microsoft.EntityFrameworkCore;
using SqlBundle.Models;
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<SqlContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ConnectPostgre")));
builder.Services.AddTransient<BundleRepo>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
