using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        CheckedOut,
        Cancelled,
        NoShow
    }

    public enum PaymentStatus
    {
        Pending,
        Partial,
        Paid,
        Refunded
    }

    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required, MaxLength(20)]
        [Display(Name = "Booking Reference")]
        public string BookingReference { get; set; } = string.Empty;

        [Required]
        public int GuestId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Actual Check-in")]
        public DateTime? ActualCheckIn { get; set; }

        [Display(Name = "Actual Check-out")]
        public DateTime? ActualCheckOut { get; set; }

        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; } = 1;

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Room Fare")]
        public decimal RoomFare { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Extra Charges")]
        public decimal ExtraCharges { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Discount")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Booking Status")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [MaxLength(500)]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(200)]
        public string? CreatedBy { get; set; }

        // Navigation
        public Guest Guest { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public Invoice? Invoice { get; set; }

        [NotMapped]
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
    }
}
