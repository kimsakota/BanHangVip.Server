using BanHangVip.Backend.Models;
using BanHangVip.Server.Data;
using BanHangVip.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanHangVip.Server.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= GET DATA =================

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer) // ⭐ Include Customer
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer) // ⭐ Include Customer
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return order;
        }

        // GET: api/Orders/Pending
        // 1. App Người bán gọi: /api/Orders/Pending (Không truyền gì -> customerId = null -> Lấy HẾT)
        // 2. Web Khách hàng gọi: /api/Orders/Pending?customerId=5 (Có truyền -> Lấy riêng khách số 5)
        [HttpGet("Pending")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPendingOrders([FromQuery] int? customerId)
        {
            // Bước 1: Khởi tạo query bao gồm đầy đủ thông tin món và khách
            var query = _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer) // Quan trọng: App người bán cần cái này để biết ai đặt
                .Where(o => o.Status == OrderStatus.Pending);

            // Bước 2: Kiểm tra logic
            // Nếu có customerId (Khách hàng đang xem) -> Chỉ lọc đơn của họ
            if (customerId.HasValue && customerId.Value > 0)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }
            // Nếu customerId = null (App người bán) -> Bỏ qua dòng trên, nghĩa là LẤY TẤT CẢ

            // Bước 3: Trả về kết quả (Mới nhất lên đầu)
            return await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Orders/Delivered?page=1&pageSize=20
        [HttpGet("Delivered")]
        public async Task<ActionResult<IEnumerable<Order>>> GetDeliveredOrders(int page = 1, int pageSize = 20)
        {
            // Đảm bảo page không nhỏ hơn 1
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .Where(o => o.Status == OrderStatus.Delivered)
                .OrderByDescending(o => o.CreatedAt) // Sắp xếp mới nhất lên đầu TRƯỚC khi cắt trang
                .Skip((page - 1) * pageSize)         // Bỏ qua các đơn hàng của trang trước
                .Take(pageSize)                      // Lấy số lượng đơn hàng giới hạn (20)
                .ToListAsync();
        }

        // GET: api/Orders/Unpaid
        [HttpGet("Unpaid")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUnpaidOrders()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .Where(o => o.Status == OrderStatus.Delivered && !o.IsPaid)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // ================= CRUD =================

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder(Order order)
        {
            // ⭐ Kiểm tra CustomerId có tồn tại không
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == order.CustomerId);
            if (!customerExists)
                return BadRequest($"Không tìm thấy khách hàng với ID: {order.CustomerId}");

            order.CreatedAt = DateTime.Now;
            order.Status = OrderStatus.Pending;
            order.IsPaid = false;

            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    item.OrderId = order.Id;
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load lại Customer để trả về đầy đủ thông tin
            await _context.Entry(order).Reference(o => o.Customer).LoadAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/Orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.Id) return BadRequest();

            // Xóa Items cũ để cập nhật Items mới
            var existingItems = _context.OrderItems.Where(i => i.OrderId == id);
            _context.OrderItems.RemoveRange(existingItems);

            _context.Entry(order).State = EntityState.Modified;

            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    item.OrderId = id;
                    _context.OrderItems.Add(item);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ================= LOGIC NGHIỆP VỤ =================

        // POST: api/Orders/Deliver/{id}
        [HttpPost("Deliver/{id}")]
        public async Task<IActionResult> DeliverOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound("Không tìm thấy đơn hàng");

            if (order.Status == OrderStatus.Delivered)
                return BadRequest("Đơn hàng này đã giao rồi.");

            // 1. Cập nhật trạng thái
            order.Status = OrderStatus.Delivered;

            // 2. Ghi log lịch sử xuất kho
            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    _context.HistoryItems.Add(new HistoryItem
                    {
                        Type = "DELIVERY",
                        ProductId = item.ProductId, // ⭐ Lưu ProductId
                        ProductName = item.ProductName,
                        Weight = item.Weight,
                        Price = item.Price,
                        Timestamp = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // POST: api/Orders/ProcessPayment/{customerId}
        [HttpPost("ProcessPayment/{customerId}")]
        public async Task<IActionResult> ProcessPayment(int customerId)
        {
            // ⭐ Kiểm tra Customer có tồn tại không
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return NotFound($"Không tìm thấy khách hàng với ID: {customerId}");

            var unpaidOrders = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == customerId
                            && o.Status == OrderStatus.Delivered
                            && !o.IsPaid)
                .ToListAsync();

            if (!unpaidOrders.Any())
                return NotFound($"Không tìm thấy đơn nợ nào của khách: {customer.Name}");

            var paymentTime = DateTime.Now;

            foreach (var order in unpaidOrders)
            {
                // 1. Đánh dấu đã trả
                order.IsPaid = true;
                order.PaidAt = paymentTime;

                // 2. Ghi log lịch sử thu tiền
                if (order.Items != null)
                {
                    foreach (var item in order.Items)
                    {
                        _context.HistoryItems.Add(new HistoryItem
                        {
                            Type = "PAYMENT",
                            ProductId = item.ProductId, // ⭐ Lưu ProductId
                            ProductName = item.ProductName,
                            Weight = item.Weight,
                            Price = item.Price,
                            Timestamp = paymentTime
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = $"Đã thanh toán {unpaidOrders.Count} đơn hàng cho {customer.Name}",
                ProcessedCount = unpaidOrders.Count,
                CustomerName = customer.Name
            });
        }
    }
}