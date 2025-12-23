using Microsoft.EntityFrameworkCore;
using MyTodoList.Data; // ApplicationDbContext burada olduğu için gerekli

var builder = WebApplication.CreateBuilder(args);

// --- 1. SESSION (OTURUM) AYARLARI ---
// Hosting'de çalışması için bu ayarlar kritiktir.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // 60 dakika açık kalsın
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // <--- BU ÇOK ÖNEMLİ! Hostinglerin çerez engellemesini aşar.
});

builder.Services.AddControllersWithViews();

// --- 2. VERİTABANI BAĞLANTISI ---

// YÖNTEM A: Dosyadan Oku (Standart)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// YÖNTEM B: (Eğer üstteki hata verirse, üsttekini sil ve alttaki yorumu açıp bilgilerini yaz)
// var connectionString = "Server=SUNUCU_IP_ADRESI,1433;Database=VERITABANI_ADI;User Id=KULLANICI_ADI;Password=SIFRE;TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// --- 3. UYGULAMA AYARLARI ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// DİKKAT: UseSession, UseRouting'den sonra, MapControllerRoute'dan önce olmalı.
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}"); // Açılış sayfası Login

app.Run();