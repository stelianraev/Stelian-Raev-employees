using EmployeeExtractor.Services;
using EmployeeExtractor.Configuration;

var builder = WebApplication.CreateBuilder(args);

var serviceConfig= builder.Configuration.GetSection("ServiceConfig");

// Add services to the container.
builder.Services.Configure<ServiceConfiguration>(serviceConfig);
builder.Services.AddSingleton<IFileParser, FileParser>();
builder.Services.AddSingleton<Engine>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
