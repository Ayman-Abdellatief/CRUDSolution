using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using Services;
 

var builder = WebApplication.CreateBuilder(args);

var licenseName = builder.Configuration["EPPlus:LicenseName"];

ExcelPackage.License.SetNonCommercialPersonal(licenseName);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

builder.Services.AddDbContext<PersonsDbContext>(otpions =>
{
    otpions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

if(!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
   
}
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot",wkhtmltopdfRelativePath: "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
