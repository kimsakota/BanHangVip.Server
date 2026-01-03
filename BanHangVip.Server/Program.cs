using BanHangVip.Server.Data;
using BanHangVip.Server.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var connectionString = "Server=103.28.36.36;Database=nhkimi2o_quanlybanhang;User Id=nhkimi2o_kimsakota1;Password=Cute123_VN@;TrustServerCertificate=True;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => true) // Cho phép mọi nguồn (App, Web...)
            .AllowCredentials()); // Bắt buộc phải có dòng này cho SignalR
});

//Session and HttpContextAccessor
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian tồn tại của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Cấu hình Swagger/OpenAPI (Để test API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Cấu hình môi trường Development
if (app.Environment.IsDevelopment())
{
    // Bật Swagger UI khi chạy ở chế độ Debug/Dev
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 2. Map cho API Controllers (các class có attribute [ApiController])
app.MapControllers();

app.MapHub<OrderHub>("/orderHub");

app.Run();
