using MusicStore.DomainLayer.Repositories.Abstraction;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;
using MusicStore.InfrastructureLayer.Data;
using MusicStore.InfrastructureLayer.Repositories.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.InfrastructureLayer.UnitOfWork.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext context)
        {
            _db = context;
            Category = new CategoryRepository(_db);
            sp_Call = new SPCallRepository(_db);
            CoverTypeRepository = new CoverTypeRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            AppUser = new AppUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);

        }


        public ICategoryRepository Category { get; private set; }

        public ISPCallRepository sp_Call { get; private set; }
        public ICoverTypeRepository CoverTypeRepository { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
         
        public void Commit()
        {
            _db.SaveChanges();
        }
    }
}
