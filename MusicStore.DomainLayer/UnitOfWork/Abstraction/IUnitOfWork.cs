using MusicStore.DomainLayer.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

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
        void Commit();
    }
}
