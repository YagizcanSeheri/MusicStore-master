using MusicStore.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.DomainLayer.Repositories.Abstraction
{
    public interface IProductRepository:IBaseRepository<Product>
    {
        void Update(Product product);
    }
}
