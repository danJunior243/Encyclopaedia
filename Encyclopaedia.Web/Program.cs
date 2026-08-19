using Encyclopaedia.Core.Entities;
using Encyclopaedia.Core.Enums;
using Encyclopaedia.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ──
builder.Services.AddControllersWithViews();

// ── Base de données ──
// ── Base de données ──
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (databaseUrl != null)
{
    // Convertir l'URL PostgreSQL au format Npgsql
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

builder.Services.AddDbContext<EncyclopaediaDbContext>(options =>
    options.UseNpgsql(connectionString));// ── Identity ──
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<EncyclopaediaDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// ── Pipeline ──
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ── Migration automatique ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EncyclopaediaDbContext>();
    db.Database.Migrate();
}

// ── Créer l'admin par défaut ──
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = "admin@encyclopaedia.com";
    var adminExists = await userManager.FindByEmailAsync(adminEmail);

    // si l'administrateur n'existe pas, on le crée avec un mot de passe par défaut. Il est recommandé de changer
    // ce mot de passe après la première connexion pour des raisons de sécurité.
    if (adminExists == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "Administrateur",
            Role = UserRole.Admin,
            IsActive = true
        };

        await userManager.CreateAsync(admin, "Admin@12345");
    }
}

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "article",
    pattern: "article/{slug}",
    defaults: new { controller = "Article", action = "Index" }
);

app.Run();

