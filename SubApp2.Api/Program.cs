
using Microsoft.EntityFrameworkCore;
using SubApp2.Api.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "studyroom-api.db");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
