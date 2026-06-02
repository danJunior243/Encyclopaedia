using Encyclopaedia.Core.Entities;
using Encyclopaedia.Core.Enums;
using Encyclopaedia.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ──
builder.Services.AddControllersWithViews();

// ── Base de données ──
builder.Services.AddDbContext<EncyclopaediaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ── Identity ──
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

