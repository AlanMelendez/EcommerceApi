using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(p => p.Category) //Include tells EF Core to also load related entities.
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        //AsNoTracking() is faster when we only read data and do not update it.

        return await _context.Products
           .AsNoTracking()
           .Include(product => product.Category)
           .OrderBy(product => product.Name)
           .ToListAsync(cancellationToken);
    }


    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        product.SoftDelete(null);
        _context.Products.Update(product);
    }

    public async Task<PagedResult<Product>> GetPagedAsync(
    ProductQueryParameters parameters,
    CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking() //It tells EF Core that we don't want to track changes for the entities returned by this query. This can improve performance for read-only queries.
            .Include(product => product.Category)
            .AsQueryable(); // It allows us to build a query dynamically based on the provided parameters.

        // Filtering by name or description if a search term is provided
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(product =>
                product.Name.Contains(search) ||
                (product.Description != null && product.Description.Contains(search)));
        }

        if (parameters.CategoryId.HasValue)
        {
            query = query.Where(product =>
                product.CategoryId == parameters.CategoryId.Value);
        }

        if (parameters.MinPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price >= parameters.MinPrice.Value);
        }

        if (parameters.MaxPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price <= parameters.MaxPrice.Value);
        }

        // Evaluate the sort direction and apply sorting based on the provided parameters
        var isDescending = parameters.SortDirection?.ToLowerInvariant() == "desc";

        query = parameters.SortBy?.ToLowerInvariant() switch
        {
            "price" => isDescending
                ? query.OrderByDescending(product => product.Price)
                : query.OrderBy(product => product.Price),

            "stock" => isDescending
                ? query.OrderByDescending(product => product.Stock)
                : query.OrderBy(product => product.Stock),

            "createdat" => isDescending
                ? query.OrderByDescending(product => product.CreatedAt)
                : query.OrderBy(product => product.CreatedAt),


            // Default sorting by name if no valid sortBy parameter is provided
            _ => isDescending
                ? query.OrderByDescending(product => product.Name)
                : query.OrderBy(product => product.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize) // Skip the items for previous pages ex. if pageNumber = 2 and pageSize = 10, it will skip the first 10 items.
            .Take(parameters.PageSize) // Take the items for the current page ex. if pageNumber = 2 and pageSize = 10, it will take the next 10 item OF the query.
            .ToListAsync(cancellationToken);

        return PagedResult<Product>.Create(
            items,
            parameters.PageNumber,
            parameters.PageSize,
            totalCount);
    }
}