using HotelBookingSystem.Data;
using HotelBookingSystem.Services;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;
        private readonly IConfiguration _config;

        public InvoiceController(ApplicationDbContext context, IBookingService bookingService, IConfiguration config)
        {
            _context = context;
            _bookingService = bookingService;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Booking).ThenInclude(b => b.Guest)
                .Include(i => i.Booking).ThenInclude(b => b.Room)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Booking).ThenInclude(b => b.Guest)
                .Include(i => i.Booking).ThenInclude(b => b.Room)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null) return NotFound();

            var vm = new InvoiceViewModel
            {
                Invoice = invoice,
                Booking = invoice.Booking,
                Guest = invoice.Booking.Guest,
                Room = invoice.Booking.Room,
                HotelName = _config["AppSettings:HotelName"] ?? "Grand Palace Hotel",
                HotelAddress = _config["AppSettings:HotelAddress"] ?? "",
                HotelPhone = _config["AppSettings:HotelPhone"] ?? "",
                HotelEmail = _config["AppSettings:HotelEmail"] ?? ""
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(int bookingId)
        {
            var invoice = await _bookingService.GenerateInvoiceAsync(bookingId);
            TempData["Success"] = $"Invoice {invoice.InvoiceNumber} generated.";
            return RedirectToAction(nameof(Details), new { id = invoice.InvoiceId });
        }

        public async Task<IActionResult> Print(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Booking).ThenInclude(b => b.Guest)
                .Include(i => i.Booking).ThenInclude(b => b.Room)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null) return NotFound();

            var vm = new InvoiceViewModel
            {
                Invoice = invoice,
                Booking = invoice.Booking,
                Guest = invoice.Booking.Guest,
                Room = invoice.Booking.Room,
                HotelName = _config["AppSettings:HotelName"] ?? "Grand Palace Hotel",
                HotelAddress = _config["AppSettings:HotelAddress"] ?? "",
                HotelPhone = _config["AppSettings:HotelPhone"] ?? "",
                HotelEmail = _config["AppSettings:HotelEmail"] ?? ""
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Booking)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
            if (invoice == null) return NotFound();

            invoice.IsPaid = true;
            invoice.AmountPaid = invoice.GrandTotal;
            invoice.BalanceDue = 0;
            invoice.Booking.AmountPaid = invoice.GrandTotal;
            invoice.Booking.PaymentStatus = Models.PaymentStatus.Paid;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Invoice marked as paid.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
