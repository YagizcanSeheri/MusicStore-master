using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MusicStore.DomainLayer.Entities
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }


        [Key]
        public int Id { get; set; }
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }


        public int Count { get; set; }

        [NotMapped]
        public double Price { get; set; }

       

    }
}
