using AutoMapper;
using Ecommerce.Application.Common.Caching;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Products;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var parameters = new ProductQueryParameters(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.SortBy,
            request.SortDirection);

        var version = await _cacheService.GetVersionAsync(
            CacheKeys.ProductsVersion,
            cancellationToken);

        var cacheKey = CacheKeys.ProductList(parameters, version);

        var cachedResult = await _cacheService.GetAsync<PagedResult<ProductResponse>>(
            cacheKey,
            cancellationToken);

        if (cachedResult is not null)
        {
            return Result<PagedResult<ProductResponse>>.Success(cachedResult);
        }

        var pagedProducts = await _productRepository.GetPagedAsync(
            parameters,
            cancellationToken);

        var productResponses = _mapper.Map<IReadOnlyList<ProductResponse>>(
            pagedProducts.Items);

        var response = PagedResult<ProductResponse>.Create(
            productResponses,
            pagedProducts.PageNumber,
            pagedProducts.PageSize,
            pagedProducts.TotalCount);

        await _cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return Result<PagedResult<ProductResponse>>.Success(response);
    }
}