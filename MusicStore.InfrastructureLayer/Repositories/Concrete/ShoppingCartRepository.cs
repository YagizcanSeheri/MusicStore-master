using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.Repositories.Abstraction;
using MusicStore.InfrastructureLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicStore.InfrastructureLayer.Repositories.Concrete
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart>,IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _db.Update(shoppingCart);
       
        }
    }
}
