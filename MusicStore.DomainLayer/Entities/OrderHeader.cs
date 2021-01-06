using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MusicStore.DomainLayer.Entities
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime ShippingDate { get; set; }

        [Required]
        public double OrderTotal { get; set; }

        public string TrackingNumber { get; set; }

        public string Carrier { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime PaymentDueDate { get; set; }

        public string TransactionId { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string PostCode { get; set; }

        public string Name { get; set; }
    }
}
