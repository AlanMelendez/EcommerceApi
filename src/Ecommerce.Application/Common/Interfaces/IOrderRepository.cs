using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Common.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);

        Task AddAsync(Order order, CancellationToken cancellationToken);

        void Update(Order order);

        void Delete(Order order);
    }
}
