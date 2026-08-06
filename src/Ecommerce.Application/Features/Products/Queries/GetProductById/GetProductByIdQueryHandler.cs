using AutoMapper;
using Ecommerce.Application.Common.Caching;
using Ecommerce.Application.Common.Errors;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Products;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var version = await _cacheService.GetVersionAsync(
            CacheKeys.ProductsVersion,
            cancellationToken);

        var cacheKey = CacheKeys.ProductById(request.Id, version);

        var cachedProduct = await _cacheService.GetAsync<ProductResponse>(
            cacheKey,
            cancellationToken);

        if (cachedProduct is not null)
        {
            return Result<ProductResponse>.Success(cachedProduct);
        }

        var product = await _productRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result<ProductResponse>.Failure(ProductErrors.NotFound);
        }

        var response = _mapper.Map<ProductResponse>(product);

        await _cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Result<ProductResponse>.Success(response);
    }
}