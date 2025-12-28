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
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Orders/Pending
        [HttpGet("Pending")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPendingOrders()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == OrderStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Orders/Delivered
        [HttpGet("Delivered")]
        public async Task<ActionResult<IEnumerable<Order>>> GetDeliveredOrders()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == OrderStatus.Delivered)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Orders/Unpaid
        [HttpGet("Unpaid")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUnpaidOrders()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == OrderStatus.Delivered && !o.IsPaid)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // ================= CRUD =================

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder(Order order)
        {
            if (string.IsNullOrEmpty(order.Id)) order.Id = Guid.NewGuid().ToString();
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

            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        }

        // PUT: api/Orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, Order order)
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

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
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
        public async Task<IActionResult> DeliverOrder(string id)
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
                        Id = $"DEL-{DateTime.Now.Ticks}-{item.ProductId}",
                        Type = "DELIVERY",
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

        // POST: api/Orders/ProcessPayment
        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] string customerName)
        {
            var unpaidOrders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerName == customerName
                            && o.Status == OrderStatus.Delivered
                            && !o.IsPaid)
                .ToListAsync();

            if (!unpaidOrders.Any())
                return NotFound($"Không tìm thấy đơn nợ nào của khách: {customerName}");

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
                            Id = $"PAY-{DateTime.Now.Ticks}-{item.ProductId}",
                            Type = "PAYMENT",
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
                Message = $"Đã thanh toán {unpaidOrders.Count} đơn hàng cho {customerName}",
                ProcessedCount = unpaidOrders.Count
            });
        }
    }
}