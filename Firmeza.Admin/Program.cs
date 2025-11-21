using Microsoft.AspNetCore.Identity;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Firmeza.Core.Interfaces;
using Firmeza.Infrastructure.Repositories;
using Firmeza.Application.Interfaces;
using Firmeza.Infrastructure.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
   ?? throw new InvalidOperationException("Conection String 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Identity con Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>() // IMPORTANTE: Agregar soporte para roles
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Registrar servicios de importación
builder.Services.AddScoped<IExcelParserService, ExcelParserService>();
builder.Services.AddScoped<IImportDataService, ImportDataService>();

// Registrar servicio de PDF
builder.Services.AddScoped<IPdfService, PdfService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Inicializar Roles y Usuario Administrador
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar roles y usuario administrador");
    }
}

// Configure the HTTP request pipeline.
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

app.MapRazorPages();

app.Run();

async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Crear roles
    string[] roleNames = { "Administrator", "Client" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"✓ Rol '{roleName}' creado correctamente.");
        }
    }

    // Crear usuario administrador por defecto
    var adminEmail = "admin@firmeza.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, "Admin@123");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Administrator");
            Console.WriteLine($"✓ Usuario administrador creado:");
            Console.WriteLine($"  Email: {adminEmail}");
            Console.WriteLine($"  Password: Admin@123");
        }
        else
        {
            Console.WriteLine($"✗ Error al crear administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}