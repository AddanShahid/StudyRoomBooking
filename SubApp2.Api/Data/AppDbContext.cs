
using Microsoft.EntityFrameworkCore;
using SubApp2.Api.Models;

namespace SubApp2.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; } = null!;
}
