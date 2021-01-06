using MusicStore.DomainLayer.Entities;

namespace MusicStore.DomainLayer.Repositories.Abstraction
{
    public interface IOrderHeaderRepository : IBaseRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
    }
}
