using Ecommerce.Application.Common.Interfaces;

namespace Ecommerce.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}