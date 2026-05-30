using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Update(Category category)
    {
        throw new NotImplementedException();
    }

    public void Delete(Category category)
    {
        throw new NotImplementedException();
    }
}