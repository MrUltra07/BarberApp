using BarberApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // PostgreSQL ba�lant�s�

// Configure Authentication and Authorization
builder.Services.AddAuthentication("SessionScheme")
	.AddCookie("SessionScheme", options =>
	{
		options.LoginPath = "/admin/login";           // Giri� sayfas�
		options.AccessDeniedPath = "/admin/accessdenied"; // Yetkisiz eri�im sayfas�
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie ya�am s�resi
	});

// Configure Session Middleware
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
	options.Cookie.HttpOnly = true;                // G�venlik i�in HTTPOnly cookie
	options.Cookie.IsEssential = true;             // GDPR uyumlulu�u
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error"); // �retim ortam�nda hata y�netimi
	app.UseHsts();                          // HSTS deste�i
}

app.UseHttpsRedirection(); // HTTPS y�nlendirmesi
app.UseStaticFiles();      // Statik dosyalara eri�im

app.UseRouting();

app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();  // Authorization middleware

app.UseSession();        // Session middleware

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
