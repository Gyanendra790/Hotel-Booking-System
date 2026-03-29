using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var vm = new DashboardViewModel
            {
                TotalRooms = await _context.Rooms.CountAsync(),
                AvailableRooms = await _context.Rooms.CountAsync(r => r.Status == RoomStatus.Available),
                OccupiedRooms = await _context.Rooms.CountAsync(r => r.Status == RoomStatus.Occupied),
                TodayCheckIns = await _context.Bookings.CountAsync(b => b.CheckInDate == today && b.Status != BookingStatus.Cancelled),
                TodayCheckOuts = await _context.Bookings.CountAsync(b => b.CheckOutDate == today && b.Status != BookingStatus.Cancelled),
                PendingBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
                TodayRevenue = await _context.Bookings
                    .Where(b => b.CreatedAt.Date == today && b.Status != BookingStatus.Cancelled)
                    .SumAsync(b => (decimal?)b.TotalAmount) ?? 0,
                MonthlyRevenue = await _context.Bookings
                    .Where(b => b.CreatedAt >= monthStart && b.Status != BookingStatus.Cancelled)
                    .SumAsync(b => (decimal?)b.TotalAmount) ?? 0,
                RecentBookings = await _context.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(8)
                    .ToListAsync(),
                TodayArrivals = await _context.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .Where(b => b.CheckInDate == today && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
                    .ToListAsync(),
                TodayDepartures = await _context.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .Where(b => b.CheckOutDate == today && b.Status == BookingStatus.CheckedIn)
                    .ToListAsync()
            };

            // Room type occupancy
            var roomTypes = Enum.GetValues<RoomType>();
            foreach (var rt in roomTypes)
            {
                var occupied = await _context.Bookings
                    .CountAsync(b => b.Room.RoomType == rt && b.Status == BookingStatus.CheckedIn);
                vm.RoomTypeOccupancy[rt.ToString()] = occupied;
            }

            // Monthly revenue last 6 months
            for (int i = 5; i >= 0; i--)
            {
                var month = today.AddMonths(-i);
                var start = new DateTime(month.Year, month.Month, 1);
                var end = start.AddMonths(1);
                var rev = await _context.Bookings
                    .Where(b => b.CreatedAt >= start && b.CreatedAt < end && b.Status != BookingStatus.Cancelled)
                    .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;
                var cnt = await _context.Bookings
                    .CountAsync(b => b.CreatedAt >= start && b.CreatedAt < end && b.Status != BookingStatus.Cancelled);
                vm.MonthlyRevenueChart.Add(new MonthlyRevenueData
                {
                    Month = month.ToString("MMM yyyy"),
                    Revenue = rev,
                    Bookings = cnt
                });
            }

            return View(vm);
        }
    }
}
