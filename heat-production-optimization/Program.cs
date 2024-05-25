using heat_production_optimization;
using heat_production_optimization.Models;
using System.Globalization;
using OfficeOpenXml;

CultureInfo customCulture = new CultureInfo("da-DK");
CultureInfo.DefaultThreadCurrentCulture = customCulture;
CultureInfo.DefaultThreadCurrentUICulture = customCulture;

var builder = WebApplication.CreateBuilder(args);

// This must stay for the excel reading and writing to work in debug mode
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<SourceDataDbContext>();

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DoubleModelBinderProvider());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
