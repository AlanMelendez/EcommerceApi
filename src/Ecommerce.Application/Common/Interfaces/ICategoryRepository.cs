using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);

        Task AddAsync(Category category, CancellationToken cancellationToken);

        void Update(Category category);

        void Delete(Category category);
    }
}
