using MusicStore.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.ApplicationLayer.ViewModels
{
    public class CartVM
    {
        public IEnumerable<ShoppingCart> CartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
