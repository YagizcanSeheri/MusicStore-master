using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.Repositories.Abstraction;
using MusicStore.InfrastructureLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicStore.InfrastructureLayer.Repositories.Concrete
{
    public class OrderHeaderRepository:BaseRepository<OrderHeader>,IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.Update(orderHeader);
       
        }
    }
}
