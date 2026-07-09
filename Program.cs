using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Options;
using ProductManagement.Repositories;
using ProductManagement.Services;

var builder = WebApplication.CreateBuilder(args);

//1. Register Database Context (Default to scoped)
// We extract the connection string from the appsettings.json and pass it to the EF SQL Server engine.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Cookie Registration for authentication
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Redirect paths for unauthorized access and login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


//Register repository as Scoped as it aligns with DbConext lifecycle.
//This ensures that the repository is created once per request and disposed of at the end of the request.
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register for excel service
builder.Services.AddTransient<IExcelExportService, ExcelExportService>();

builder.Services.Configure<SiteInfo>(builder.Configuration.GetSection("SiteInfo"));


// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}")
    .WithStaticAssets();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // 1. Ensure the Admin Role exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 2. Ensure a default Admin user exists
    string adminEmail = "admin@project.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        var createAdminResult = await userManager.CreateAsync(newAdmin, "AdminPassword123!");

        if (createAdminResult.Succeeded)
        {
            // 3. Bind the user to the Admin role
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}

app.Run();
