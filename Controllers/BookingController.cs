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
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;

        public BookingController(ApplicationDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(string? status, string? search)
        {
            var query = _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, out var bs))
                query = query.Where(b => b.Status == bs);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b =>
                    b.BookingReference.Contains(search) ||
                    b.Guest.FirstName.Contains(search) ||
                    b.Guest.LastName.Contains(search) ||
                    b.Room.RoomNumber.Contains(search));

            ViewBag.StatusFilter = status;
            ViewBag.Search = search;
            ViewBag.Statuses = Enum.GetValues<BookingStatus>();
            return View(await query.OrderByDescending(b => b.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .Include(b => b.Invoice)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? guestId, int? roomId)
        {
            var vm = new BookingCreateViewModel
            {
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1),
                GuestId = guestId,
                RoomId = roomId ?? 0,
                AvailableRooms = await _context.Rooms
                    .Where(r => r.Status == RoomStatus.Available)
                    .OrderBy(r => r.RoomNumber).ToListAsync(),
                ExistingGuests = await _context.Guests.OrderBy(g => g.FirstName).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel vm)
        {
            vm.AvailableRooms = await _context.Rooms
                .Where(r => r.Status == RoomStatus.Available)
                .OrderBy(r => r.RoomNumber).ToListAsync();
            vm.ExistingGuests = await _context.Guests.OrderBy(g => g.FirstName).ToListAsync();

            if (vm.CheckInDate >= vm.CheckOutDate)
            {
                ModelState.AddModelError("", "Check-out must be after check-in date.");
                return View(vm);
            }

            int guestId = vm.GuestId ?? 0;

            // Create new guest if not existing
            if (guestId == 0)
            {
                if (string.IsNullOrEmpty(vm.GuestFirstName) || string.IsNullOrEmpty(vm.GuestEmail))
                {
                    ModelState.AddModelError("", "Please select an existing guest or enter new guest details.");
                    return View(vm);
                }

                var newGuest = new Guest
                {
                    FirstName = vm.GuestFirstName!,
                    LastName = vm.GuestLastName ?? "",
                    Email = vm.GuestEmail!,
                    Phone = vm.GuestPhone ?? ""
                };
                _context.Guests.Add(newGuest);
                await _context.SaveChangesAsync();
                guestId = newGuest.GuestId;
            }

            // Check availability
            var available = await _bookingService.GetAvailableRoomsAsync(vm.CheckInDate, vm.CheckOutDate);
            if (!available.Any(r => r.RoomId == vm.RoomId))
            {
                ModelState.AddModelError("", "The selected room is not available for these dates.");
                return View(vm);
            }

            var booking = new Booking
            {
                GuestId = guestId,
                RoomId = vm.RoomId,
                CheckInDate = vm.CheckInDate,
                CheckOutDate = vm.CheckOutDate,
                NumberOfGuests = vm.NumberOfGuests,
                SpecialRequests = vm.SpecialRequests,
                Status = BookingStatus.Confirmed,
                CreatedBy = User.Identity?.Name
            };

            await _bookingService.CreateBookingAsync(booking);
            TempData["Success"] = $"Booking {booking.BookingReference} created successfully.";
            return RedirectToAction(nameof(Details), new { id = booking.BookingId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id)
        {
            var ok = await _bookingService.CheckInGuestAsync(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Guest checked in successfully." : "Check-in failed.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int id)
        {
            var ok = await _bookingService.CheckOutGuestAsync(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Guest checked out. Invoice generated." : "Check-out failed.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Booking cancelled.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Booking booking)
        {
            var existing = await _context.Bookings.FindAsync(booking.BookingId);
            if (existing == null) return NotFound();

            existing.NumberOfGuests = booking.NumberOfGuests;
            existing.CheckInDate = booking.CheckInDate;
            existing.CheckOutDate = booking.CheckOutDate;
            existing.SpecialRequests = booking.SpecialRequests;
            existing.Notes = booking.Notes;
            existing.ExtraCharges = booking.ExtraCharges;
            existing.Discount = booking.Discount;
            existing.AmountPaid = booking.AmountPaid;
            existing.PaymentStatus = booking.PaymentStatus;
            existing.Status = booking.Status;

            // Recalculate fare
            var (fare, tax, _) = await _bookingService.CalculateFareAsync(existing.RoomId, existing.CheckInDate, existing.CheckOutDate);
            existing.RoomFare = fare;
            existing.TaxAmount = tax;
            existing.TotalAmount = fare + tax + existing.ExtraCharges - existing.Discount;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Booking updated.";
            return RedirectToAction(nameof(Details), new { id = booking.BookingId });
        }
    }
}
