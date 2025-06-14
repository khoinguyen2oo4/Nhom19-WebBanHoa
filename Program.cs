using Microsoft.EntityFrameworkCore;
using Nhom19_WebBanHoa.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Nhom19_WebBanHoa.Hubs;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

// K?t n?i database
builder.Services.AddDbContext<FlowerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// C?u hình Session (gi? l?i t? c? hai)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // H?t h?n session sau 30 phút
});

// C?u hình Localization (hi?n th? ??nh d?ng ti?n VN?)
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "vi-VN", "en-US" };
    var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();

    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ?ã chu?n – không c?n MapStaticAssets

app.UseRouting();
app.UseSession();              // Session ph?i ??t tr??c Authorization
app.UseRequestLocalization();  // Áp d?ng ??nh d?ng ti?n t?/language
app.UseAuthorization();
app.UseAuthentication(); // ?? thêm dòng này

// Route m?c ??nh
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<Nhom19_WebBanHoa.Hubs.ChatHub>("/chathub");

app.Run();
