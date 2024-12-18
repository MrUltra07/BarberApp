using BarberApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // PostgreSQL baðlantýsý

// Add Authentication and Authorization
builder.Services.AddAuthentication("SessionScheme")
    .AddCookie("SessionScheme", options =>
    {
        options.LoginPath = "/admin/login"; // Giriþ sayfasý
        options.AccessDeniedPath = "/admin/accessdenied"; // Yetkisiz eriþim sayfasý
    });

// Add Session Middleware
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true;                // Güvenlik için HTTPOnly cookie
    options.Cookie.IsEssential = true;             // GDPR için gerekli
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseStaticFiles();
app.UseRouting();

app.UseSession();        // Session middleware
app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();  // Authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
