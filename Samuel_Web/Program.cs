using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Samuel_Web.DataAccess.Data;
using Samuel_Web.DataAccess.DBInitializer;
using Samuel_Web.DataAccess.Repository;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using Samuel_Web.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();   
// her we get connection string from appsettings.json 
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("Stripe"));

// add dbcontext to the services container it means if any class need AppDbContext it will be provided by di
builder.Services.AddDbContext<AppDbContext>(opt=> opt.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser , IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IUnitOfWork, Unitofwork>();
builder.Services.AddScoped<IDBInitializer, DBInitializer>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(100);
    opt.Cookie.HttpOnly = true; 
    opt.Cookie.IsEssential = true;  
});

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Identity/Account/Login";
    opt.LogoutPath = "/Identity/Account/Logout";
    opt.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseSession();
SeedDatabase(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages();

app.Run();



void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        db.Initialize();
    }
}