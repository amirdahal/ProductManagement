using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Options;
using ProductManagement.Repositories;
using ProductManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//1. Register Database Context (Default to scoped)
// We extract the connection string from the appsettings.json and pass it to the EF SQL Server engine.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//2. Register repository as Scoped as it aligns with DbConext lifecycle.
//This ensures that the repository is created once per request and disposed of at the end of the request.
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register for excel service
builder.Services.AddTransient<IExcelExportService, ExcelExportService>();

builder.Services.Configure<SiteInfo>(builder.Configuration.GetSection("SiteInfo"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
