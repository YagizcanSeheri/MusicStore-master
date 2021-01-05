using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.Repositories.Abstraction;
using MusicStore.InfrastructureLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicStore.InfrastructureLayer.Repositories.Concrete
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(Product product)
        {
            var data = _db.Products.FirstOrDefault(x => x.Id == product.Id);
            if (data != null)
            {
                if (product.ImageUrl != null)
                {
                    data.ImageUrl = product.ImageUrl;
                }
                data.ISBN = product.ISBN;
                data.Price = product.Price;
                data.Description = product.Description;
                data.Price50 = product.Price50;
                data.Price100 = product.Price100;
                data.ListPrice = product.ListPrice;
                data.Title = product.Title;
                data.Artist = product.Artist;
                data.CategoryId = product.CategoryId;
                data.CoverTypeId = product.CoverTypeId;
            }

        }
    }
}
