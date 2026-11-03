using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity: users are stored in the same database (no email confirmation for local testing)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>();

// Send users to our own login page when they must be logged in
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});
// Repositories (DAL) used by the controllers instead of the DbContext directly
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();

var app = builder.Build();

// Create or update the database and add example data if it is empty
DbInit.Seed(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();          // finds which page/controller is requested

app.UseAuthentication();   // who is the user?
app.UseAuthorization();    // is the user allowed?

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.MapControllerRoute(
    name: "equipment",
    pattern: "Equipment/{action=Index}/{id?}",
    defaults: new { controller = "Equipment" });    
   
app.Run();


