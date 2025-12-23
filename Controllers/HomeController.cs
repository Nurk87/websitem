using Microsoft.AspNetCore.Mvc;
using MyTodoList.Data;
using MyTodoList.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTodoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. GİRİŞ EKRANI ---

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string adSoyad, string sifre)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == adSoyad && x.Password == sifre);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index");
            }

            ViewBag.Hata = "Kullanıcı adı veya şifre yanlış!";
            return View();
        }

        // --- 2. KAYIT EKRANI ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User model)
        {
            if (_context.Users.Any(x => x.Username == model.Username))
            {
                ViewBag.Hata = "Bu kullanıcı adı zaten alınmış.";
                return View();
            }

            _context.Users.Add(model);
            _context.SaveChanges();

            TempData["Basarili"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        // --- 3. ŞİFREMİ UNUTTUM ---
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username, string securityQuestion, string securityAnswer)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                ViewBag.Hata = "Böyle bir kullanıcı bulunamadı.";
                return View();
            }

            if (user.SecurityQuestion != securityQuestion ||
                user.SecurityAnswer?.ToLower().Trim() != securityAnswer?.ToLower().Trim())
            {
                ViewBag.Hata = "Güvenlik sorusu veya cevabı hatalı!";
                return View();
            }

            TempData["ResetUsername"] = username;
            return RedirectToAction("ResetPassword");
        }

        // --- 4. YENİ ŞİFRE BELİRLEME ---
        [HttpGet]
        public IActionResult ResetPassword()
        {
            var username = TempData["ResetUsername"]?.ToString();
            if (string.IsNullOrEmpty(username)) return RedirectToAction("ForgotPassword");

            ViewBag.Username = username;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string username, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Hata = "Oturum süresi dolmuş.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Hata = "Şifreler uyuşmuyor!";
                ViewBag.Username = username;
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.Password = newPassword.Trim();
                _context.SaveChanges();
                TempData["Basarili"] = "Şifreniz güncellendi!";
                return RedirectToAction("Login");
            }
            return View();
        }

        // --- 5. ANA SAYFA (LİSTE) ---
        
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            // Tamamlanmamış görevleri getir
            var aktifGorevler = _context.Todos
                                        .Where(x => x.UserId == userId && x.IsCompleted == false)
                                        .OrderBy(x => x.Saat)
                                        .ToList();

            return View(aktifGorevler);
        }

        // --- 6. GÖREV EKLEME ---
        [HttpPost]
        public IActionResult GorevEkle(string yapilacakIs, string saat)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null && !string.IsNullOrEmpty(yapilacakIs))
            {
                var yeniGorev = new TodoItem
                {
                    UserId = (int)userId,
                    YapilacakIs = yapilacakIs,
                    Saat = saat ?? "--:--",
                    IsCompleted = false
                };

                _context.Todos.Add(yeniGorev);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // --- 7. GÖREV TAMAMLA ---
        public IActionResult GorevTamamla(int id)
        {
            var gorev = _context.Todos.Find(id);
            if (gorev != null)
            {
                gorev.IsCompleted = true;
                gorev.TamamlanmaTarihi = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // --- 8. GEÇMİŞ SAYFASI ---
        public IActionResult Gecmis()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            // Tamamlanmış görevleri getir
            var bitenGorevler = _context.Todos
                                        .Where(x => x.UserId == userId && x.IsCompleted == true)
                                        .OrderByDescending(x => x.TamamlanmaTarihi)
                                        .ToList();

            return View(bitenGorevler);
        }

        // --- 9. TEK GÖREV SİLME ---
        public IActionResult GorevSil(int id)
        {
            var gorev = _context.Todos.Find(id);
            if (gorev != null)
            {
                _context.Todos.Remove(gorev);
                _context.SaveChanges();
            }
            // İstek nereden geldiyse oraya dön (Basitçe Index'e yönlendirdim)
            return RedirectToAction("Index");
        }

        // --- 10. TOPLU SİLME (GEÇMİŞ İÇİN) ---
        [HttpPost]
        public IActionResult TopluSilGecmis(List<int> silinecekIds)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            if (silinecekIds != null && silinecekIds.Any())
            {
                var silinecekler = _context.Todos
                    .Where(x => x.UserId == userId && silinecekIds.Contains(x.Id))
                    .ToList();

                if (silinecekler.Any())
                {
                    _context.Todos.RemoveRange(silinecekler);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Gecmis");
        }

        // --- 11. ÇIKIŞ YAP ---
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}