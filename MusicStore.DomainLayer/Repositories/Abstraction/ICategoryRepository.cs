using MusicStore.DomainLayer.Entities;

namespace MusicStore.DomainLayer.Repositories.Abstraction
{
    public interface ICategoryRepository:IBaseRepository<Category>
    {
        void Update(Category category);
    }
}
