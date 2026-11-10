
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var dbPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "..", "studyroombooking.db"));

if (!File.Exists(dbPath))
{
    throw new FileNotFoundException("Shared database not found.", dbPath);
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

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
