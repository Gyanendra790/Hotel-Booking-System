using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;

        public RoomController(ApplicationDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.OrderBy(r => r.Floor).ThenBy(r => r.RoomNumber).ToListAsync();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult Availability()
        {
            return View(new RoomAvailabilityViewModel
            {
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Availability(RoomAvailabilityViewModel model)
        {
            if (model.CheckInDate >= model.CheckOutDate)
            {
                ModelState.AddModelError("", "Check-out must be after check-in.");
                return View(model);
            }

            model.AvailableRooms = await _bookingService.GetAvailableRoomsAsync(model.CheckInDate, model.CheckOutDate, model.RoomType);
            model.SearchPerformed = true;
            return View(model);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public IActionResult Create() => View(new Room());

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (!ModelState.IsValid) return View(room);

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Room added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Room room)
        {
            if (!ModelState.IsValid) return View(room);

            _context.Update(room);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Room updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Room deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get fare calculation
        [HttpGet]
        public async Task<IActionResult> GetFare(int roomId, string checkIn, string checkOut)
        {
            if (!DateTime.TryParse(checkIn, out var ci) || !DateTime.TryParse(checkOut, out var co))
                return BadRequest();

            var (fare, tax, total) = await _bookingService.CalculateFareAsync(roomId, ci, co);
            int nights = (co - ci).Days;

            return Json(new { fare, tax, total, nights });
        }
    }
}
