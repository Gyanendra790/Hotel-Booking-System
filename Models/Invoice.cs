using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required, MaxLength(20)]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public int BookingId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Extra Charges")]
        public decimal ExtraCharges { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Balance Due")]
        public decimal BalanceDue { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Booking Booking { get; set; } = null!;
    }
}
