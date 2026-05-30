using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Update(Order order)
    {
        throw new NotImplementedException();
    }

    public void Delete(Order order)
    {
        throw new NotImplementedException();
    }
}