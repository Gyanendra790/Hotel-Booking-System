using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.ViewModels
{
    // ---- Auth ----
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    // ---- Admin: User Management ----
    public class CreateUserViewModel
    {
        [Required, MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        public List<string> AvailableRoles { get; set; } = new();
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        public List<string> AvailableRoles { get; set; } = new();
    }

    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    // ---- Dashboard ----
    public class DashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public int PendingBookings { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<Booking> RecentBookings { get; set; } = new();
        public List<Booking> TodayArrivals { get; set; } = new();
        public List<Booking> TodayDepartures { get; set; } = new();
        public Dictionary<string, int> RoomTypeOccupancy { get; set; } = new();
        public List<MonthlyRevenueData> MonthlyRevenueChart { get; set; } = new();
    }

    public class MonthlyRevenueData
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
    }

    // ---- Booking ----
    public class BookingCreateViewModel
    {
        public int? GuestId { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required, DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 10)]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; } = 1;

        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }

        // Guest Info (new guest)
        [Display(Name = "First Name")]
        public string? GuestFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? GuestLastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? GuestEmail { get; set; }

        [Phone]
        [Display(Name = "Phone")]
        public string? GuestPhone { get; set; }

        // Fare Summary
        public decimal PricePerNight { get; set; }
        public int NumberOfNights { get; set; }
        public decimal RoomFare { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public List<Room> AvailableRooms { get; set; } = new();
        public List<Guest> ExistingGuests { get; set; } = new();
    }

    public class RoomAvailabilityViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        public RoomType? RoomType { get; set; }
        public List<Room> AvailableRooms { get; set; } = new();
        public bool SearchPerformed { get; set; }
    }

    // ---- Invoice ----
    public class InvoiceViewModel
    {
        public Invoice Invoice { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
        public Guest Guest { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public string HotelName { get; set; } = string.Empty;
        public string HotelAddress { get; set; } = string.Empty;
        public string HotelPhone { get; set; } = string.Empty;
        public string HotelEmail { get; set; } = string.Empty;
    }
}
