using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models
{
    public enum IdType
    {
        Passport,
        NationalId,
        DriversLicense,
        Other
    }

    public class Guest
    {
        [Key]
        public int GuestId { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [Display(Name = "ID Type")]
        public IdType IdType { get; set; } = IdType.Passport;

        [MaxLength(100)]
        [Display(Name = "ID Number")]
        public string? IdNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
