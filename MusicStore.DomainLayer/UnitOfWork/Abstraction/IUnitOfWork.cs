using MusicStore.DomainLayer.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.DomainLayer.UnitOfWork.Abstraction
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category{get;}
        ISPCallRepository sp_Call { get; }
        ICoverTypeRepository CoverTypeRepository { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IAppUserRepository AppUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IOrderHeaderRepository OrderHeader { get; }
        void Commit();
    }
}
