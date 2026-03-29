using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public enum RoomType
    {
        Standard,
        Deluxe,
        Suite
    }

    public enum RoomStatus
    {
        Available,
        Occupied,
        Maintenance,
        Reserved
    }

    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required, MaxLength(10)]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Room Type")]
        public RoomType RoomType { get; set; }

        [Required]
        [Display(Name = "Floor")]
        public int Floor { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Price Per Night")]
        public decimal PricePerNight { get; set; }

        [Display(Name = "Max Occupancy")]
        public int MaxOccupancy { get; set; } = 2;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Room Status")]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [Display(Name = "Has AC")]
        public bool HasAC { get; set; } = true;

        [Display(Name = "Has WiFi")]
        public bool HasWiFi { get; set; } = true;

        [Display(Name = "Has TV")]
        public bool HasTV { get; set; } = true;

        [Display(Name = "Has Mini Bar")]
        public bool HasMiniBar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
