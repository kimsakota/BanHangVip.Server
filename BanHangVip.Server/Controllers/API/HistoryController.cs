using BanHangVip.Backend.Models;
using BanHangVip.Server.Data;
using BanHangVip.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanHangVip.Server.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/History
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoryItem>>> GetHistory()
        {
            return await _context.HistoryItems
                .Include(h => h.Product) // ⭐ Include Product
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();
        }

        // GET: api/History/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HistoryItem>> GetHistoryItem(int id)
        {
            var item = await _context.HistoryItems
                .Include(h => h.Product) // ⭐ Include Product
                .FirstOrDefaultAsync(h => h.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        // POST: api/History
        [HttpPost]
        public async Task<ActionResult<HistoryItem>> AddHistoryItem(HistoryItem item)
        {
            if (item.Timestamp == default) item.Timestamp = DateTime.Now;

            _context.HistoryItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHistoryItem), new { id = item.Id }, item);
        }

        // PUT: api/History/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHistoryItem(int id, HistoryItem item)
        {
            if (id != item.Id) return BadRequest("ID không khớp");

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.HistoryItems.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/History/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistoryItem(int id)
        {
            var item = await _context.HistoryItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.HistoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}