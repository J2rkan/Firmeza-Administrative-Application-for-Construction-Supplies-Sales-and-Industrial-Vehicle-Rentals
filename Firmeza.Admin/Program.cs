using Microsoft.AspNetCore.Identity;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Conection String 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    
    string[] roleNames = { "Administrator", "Client" };

    foreach (var roleName in roleNames)
    {
        
        var roleExist = await roleManager.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"Rol '{roleName}' creado correctamente.");
        }
    }
}