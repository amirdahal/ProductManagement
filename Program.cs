using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductManagement.Data;
using ProductManagement.Options;
using ProductManagement.Repositories;
using ProductManagement.Services;
using System.Text;

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

// Jwt authentication
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

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

//builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });


// API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Returns version info in response headers
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Read version from URL route
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV"; // Formats version as 'v1', 'v2'
        options.SubstituteApiVersionInUrl = true;
    });

// Swagger generator
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your raw valid JWT token. Example: eyJhbGciOiJIUzI1NiIsIn..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
    });
}

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

// For api
//app.MapControllers();

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
