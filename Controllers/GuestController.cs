using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    [Authorize]
    public class GuestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Guests.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(g => g.FirstName.Contains(search) ||
                                         g.LastName.Contains(search) ||
                                         g.Email.Contains(search) ||
                                         g.Phone.Contains(search));
            ViewBag.Search = search;
            return View(await query.OrderByDescending(g => g.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var guest = await _context.Guests
                .Include(g => g.Bookings).ThenInclude(b => b.Room)
                .FirstOrDefaultAsync(g => g.GuestId == id);
            if (guest == null) return NotFound();
            return View(guest);
        }

        [HttpGet]
        public IActionResult Create() => View(new Guest());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guest guest)
        {
            if (!ModelState.IsValid) return View(guest);
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Guest added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();
            return View(guest);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guest guest)
        {
            if (!ModelState.IsValid) return View(guest);
            _context.Update(guest);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Guest updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Guest deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX search for booking form
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var guests = await _context.Guests
                .Where(g => g.FirstName.Contains(term) || g.LastName.Contains(term) || g.Email.Contains(term))
                .Select(g => new { g.GuestId, fullName = g.FirstName + " " + g.LastName, g.Email, g.Phone })
                .Take(10)
                .ToListAsync();
            return Json(guests);
        }
    }
}
