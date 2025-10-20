var builder = WebApplication.CreateBuilder(args);

// 🔹 1. Servis Kayıtları
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // API'ye istek atmak için

// 🔹 2. Session servisini düzgün şekilde aktif et
builder.Services.AddDistributedMemoryCache(); // Session verisini RAM'de tutar
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 🔹 3. Ortam ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HTTP Strict Transport Security
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 4. Authentication (eğer JWT veya cookie login varsa)
app.UseAuthentication();

// 🔹 5. Authorization
app.UseAuthorization();

// 🔹 6. Session Middleware (Authorization’dan sonra gelmeli)
app.UseSession();

// 🔹 7. MVC Route yapılandırması
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
