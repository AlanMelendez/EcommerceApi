using Ecommerce.Application.Common.Models;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Common.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);

        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task<PagedResult<Product>> GetPagedAsync(
            ProductQueryParameters parameters,
            CancellationToken cancellationToken);

        void Update(Product product);

        void Delete(Product product);
    }
}
