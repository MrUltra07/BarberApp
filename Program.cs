using BarberApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // PostgreSQL baðlantýsý

// Configure Authentication and Authorization
builder.Services.AddAuthentication("SessionScheme")
	.AddCookie("SessionScheme", options =>
	{
		options.LoginPath = "/admin/login";           // Giriþ sayfasý
		options.AccessDeniedPath = "/admin/accessdenied"; // Yetkisiz eriþim sayfasý
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie yaþam süresi
	});

// Configure Session Middleware
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
	options.Cookie.HttpOnly = true;                // Güvenlik için HTTPOnly cookie
	options.Cookie.IsEssential = true;             // GDPR uyumluluðu
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error"); // Üretim ortamýnda hata yönetimi
	app.UseHsts();                          // HSTS desteði
}

app.UseHttpsRedirection(); // HTTPS yönlendirmesi
app.UseStaticFiles();      // Statik dosyalara eriþim

app.UseRouting();

app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();  // Authorization middleware

app.UseSession();        // Session middleware

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
