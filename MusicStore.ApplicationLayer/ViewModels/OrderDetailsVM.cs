using MusicStore.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.ApplicationLayer.ViewModels
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
