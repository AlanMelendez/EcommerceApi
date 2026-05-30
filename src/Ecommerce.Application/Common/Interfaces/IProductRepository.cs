using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Common.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);

        Task AddAsync(Product product, CancellationToken cancellationToken);

        void Update(Product product);

        void Delete(Product product);
    }
}
