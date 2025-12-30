using System.Diagnostics;
using BanHangVip.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace BanHangVip.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // [QUAN TRỌNG] Hàm này bị thiếu, bạn cần thêm vào để chạy trang Đơn Đang Đợi
        public IActionResult Pending()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        // [QUAN TRỌNG] Hàm này dành cho trang Thống kê (nếu bạn đã thêm menu Thống kê)
        public IActionResult Statistics()
        {
            return View();
        }

        // Action cũ của bạn (có thể giữ lại hoặc xóa nếu không dùng)
        public IActionResult Order()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}