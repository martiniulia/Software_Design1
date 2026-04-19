using FlowerShop.Data;
using FlowerShop.Middleware;
using FlowerShop.Repositories;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ), contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ), ServiceLifetime.Singleton);
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IFlowerRepository, FlowerRepository>();
builder.Services.AddScoped<IBouquetRepository, BouquetRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFloristRepository, FloristRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IFlowerService, FlowerService>();
builder.Services.AddScoped<IBouquetService, BouquetService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFloristsService, FloristsService>();
builder.Services.AddScoped<IFloristOrdersService, FloristOrdersService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<FlowerShop.Events.IEventBus, FlowerShop.Events.InMemoryEventBus>();

builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddTransient<FlowerShop.Events.IEventHandler<FlowerShop.Events.FlowerCreatedEvent>, NotificationService>();
builder.Services.AddTransient<FlowerShop.Events.IEventHandler<FlowerShop.Events.FlowerUpdatedEvent>, NotificationService>();
builder.Services.AddTransient<FlowerShop.Events.IEventHandler<FlowerShop.Events.FlowerDeletedEvent>, NotificationService>();

builder.Services.AddSingleton<FlowerShop.Export.IExportStrategy, FlowerShop.Export.JsonExportStrategy>();
builder.Services.AddSingleton<FlowerShop.Export.IExportStrategy, FlowerShop.Export.XmlExportStrategy>();
builder.Services.AddSingleton<FlowerShop.Export.IExportStrategy, FlowerShop.Export.CsvExportStrategy>();

builder.Services.AddSingleton<FlowerShop.Export.ExportStrategyFactory>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });
builder.Services.AddAuthorization();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var eventBus = app.Services.GetRequiredService<FlowerShop.Events.IEventBus>();
    var createdHandler = scope.ServiceProvider.GetRequiredService<FlowerShop.Events.IEventHandler<FlowerShop.Events.FlowerCreatedEvent>>();
    var updatedHandler = scope.ServiceProvider.GetRequiredService<FlowerShop.Events.IEventHandler<FlowerShop.Events.FlowerUpdatedEvent>>();
    var deletedHandler = scope.ServiceProvider.GetRequiredService<FlowerShop.Events.IEventHandler<FlowerShop.Events.FlowerDeletedEvent>>();

    eventBus.Subscribe(createdHandler);
    eventBus.Subscribe(updatedHandler);
    eventBus.Subscribe(deletedHandler);
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseMiddleware<SessionAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Flowers}/{action=Index}/{id?}");
app.Run();
