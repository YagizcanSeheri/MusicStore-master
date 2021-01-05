using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.Repositories.Abstraction;
using MusicStore.InfrastructureLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicStore.InfrastructureLayer.Repositories.Concrete
{
    public class CoverTypeRepository : BaseRepository<CoverType>,ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(CoverType cover)
        {
            var data = _db.CoverTypes.FirstOrDefault(x => x.Id == cover.Id);
            if (data != null)
            {
                data.Name = cover.Name;
            }

        }
    }
}
