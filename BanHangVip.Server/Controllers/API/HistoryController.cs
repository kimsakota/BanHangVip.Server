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
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();
        }

        // POST: api/History
        [HttpPost]
        public async Task<ActionResult<HistoryItem>> AddHistoryItem(HistoryItem item)
        {
            if (string.IsNullOrEmpty(item.Id)) item.Id = Guid.NewGuid().ToString();

            if (item.Timestamp == default) item.Timestamp = DateTime.Now;

            _context.HistoryItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHistory), new { id = item.Id }, item);
        }

        // DELETE: api/History/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistoryItem(string id)
        {
            var item = await _context.HistoryItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.HistoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}