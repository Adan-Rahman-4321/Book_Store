using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Data;
using Website.Models;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Book_DB_Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBCS") ?? throw new InvalidOperationException("Connection string 'DBCS' not found.")));


// Ye auto matically generate hua using identity scaffolding 
builder.Services.AddDefaultIdentity<DefaultUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddEntityFrameworkStores<Book_DB_Context>();

builder.Services.AddTransient<Website.Services.IEmailSender, EmailSender>();

builder.Services.AddScoped<UserRoleInitializer>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<Cart>(sp => Cart.GetCart(sp));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    //options.IdleTimeout = TimeSpan.FromSeconds(10);

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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}");
app.MapRazorPages();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var identityContext = services.GetRequiredService<Book_DB_Context>();
    await identityContext.Database.MigrateAsync();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An Error occured During Migration");
}


app.Run();
