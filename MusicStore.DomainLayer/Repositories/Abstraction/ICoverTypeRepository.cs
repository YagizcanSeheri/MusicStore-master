using MusicStore.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.DomainLayer.Repositories.Abstraction
{
    public interface ICoverTypeRepository : IBaseRepository<CoverType>
    {
        void Update(CoverType cover);
    }
}
