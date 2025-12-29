using BanHangVip.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanHangVip.Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang nhập mã PIN
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý khi bấm đăng nhập
        [HttpPost]
        public IActionResult Login(string pinCode)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.PinCode == pinCode);
            if (customer != null)
            {
                // Lưu thông tin vào Session
                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                HttpContext.Session.SetString("CustomerName", customer.Name ?? "Khách");
                HttpContext.Session.SetString("CustomerAvatar", customer.Avatar ?? "");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Mã PIN không đúng!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}