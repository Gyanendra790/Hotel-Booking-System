using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services
{
    public interface IBookingService
    {
        Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, RoomType? roomType = null);
        Task<(decimal roomFare, decimal tax, decimal total)> CalculateFareAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Invoice> GenerateInvoiceAsync(int bookingId);
        Task<bool> CheckInGuestAsync(int bookingId);
        Task<bool> CheckOutGuestAsync(int bookingId);
        string GenerateBookingReference();
        string GenerateInvoiceNumber();
    }

    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public BookingService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, RoomType? roomType = null)
        {
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                            b.Status != BookingStatus.CheckedOut &&
                            b.Status != BookingStatus.NoShow &&
                            b.CheckInDate < checkOut &&
                            b.CheckOutDate > checkIn)
                .Select(b => b.RoomId)
                .ToListAsync();

            var query = _context.Rooms
                .Where(r => r.Status == RoomStatus.Available && !bookedRoomIds.Contains(r.RoomId));

            if (roomType.HasValue)
                query = query.Where(r => r.RoomType == roomType.Value);

            return await query.OrderBy(r => r.RoomNumber).ToListAsync();
        }

        public async Task<(decimal roomFare, decimal tax, decimal total)> CalculateFareAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return (0, 0, 0);

            int nights = (checkOut - checkIn).Days;
            if (nights <= 0) nights = 1;

            decimal roomFare = room.PricePerNight * nights;
            decimal taxRate = _config.GetValue<decimal>("AppSettings:TaxRate", 0.12m);
            decimal tax = Math.Round(roomFare * taxRate, 2);
            decimal total = roomFare + tax;

            return (roomFare, tax, total);
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            booking.BookingReference = GenerateBookingReference();

            var (roomFare, tax, total) = await CalculateFareAsync(booking.RoomId, booking.CheckInDate, booking.CheckOutDate);
            booking.RoomFare = roomFare;
            booking.TaxAmount = tax;
            booking.TotalAmount = total + booking.ExtraCharges - booking.Discount;
            booking.CreatedAt = DateTime.Now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Invoice> GenerateInvoiceAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId)
                ?? throw new InvalidOperationException("Booking not found");

            var existing = await _context.Invoices.FirstOrDefaultAsync(i => i.BookingId == bookingId);
            if (existing != null) return existing;

            var invoice = new Invoice
            {
                InvoiceNumber = GenerateInvoiceNumber(),
                BookingId = bookingId,
                InvoiceDate = DateTime.Now,
                DueDate = booking.CheckOutDate,
                SubTotal = booking.RoomFare,
                TaxAmount = booking.TaxAmount,
                Discount = booking.Discount,
                ExtraCharges = booking.ExtraCharges,
                GrandTotal = booking.TotalAmount,
                AmountPaid = booking.AmountPaid,
                BalanceDue = booking.TotalAmount - booking.AmountPaid,
                IsPaid = booking.PaymentStatus == PaymentStatus.Paid,
                CreatedAt = DateTime.Now
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<bool> CheckInGuestAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = BookingStatus.CheckedIn;
            booking.ActualCheckIn = DateTime.Now;

            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room != null) room.Status = RoomStatus.Occupied;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckOutGuestAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = BookingStatus.CheckedOut;
            booking.ActualCheckOut = DateTime.Now;

            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room != null) room.Status = RoomStatus.Available;

            // Auto generate invoice on checkout
            await GenerateInvoiceAsync(bookingId);
            await _context.SaveChangesAsync();
            return true;
        }

        public string GenerateBookingReference()
        {
            var prefix = "BK";
            var timestamp = DateTime.Now.ToString("yyMMddHHmm");
            var random = new Random().Next(100, 999);
            return $"{prefix}{timestamp}{random}";
        }

        public string GenerateInvoiceNumber()
        {
            var prefix = "INV";
            var timestamp = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}-{timestamp}-{random}";
        }
    }
}
