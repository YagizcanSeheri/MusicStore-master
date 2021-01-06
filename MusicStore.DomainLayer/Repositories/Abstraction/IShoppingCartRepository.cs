using MusicStore.DomainLayer.Entities;

namespace MusicStore.DomainLayer.Repositories.Abstraction
{
    public interface IShoppingCartRepository:IBaseRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
    }
}
