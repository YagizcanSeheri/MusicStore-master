using MusicStore.DomainLayer.Entities;

namespace MusicStore.DomainLayer.Repositories.Abstraction
{
    public interface IOrderDetailsRepository:IBaseRepository<OrderDetails>
    {
        void Update(OrderDetails orderDetails);
    }
}
