using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.Repositories.Abstraction;
using MusicStore.InfrastructureLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicStore.InfrastructureLayer.Repositories.Concrete
{
    public class OrderDetailsRepository : BaseRepository<OrderDetails>,IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(OrderDetails orderDetails)
        {
            _db.Update(orderDetails);
       
        }
    }
}
