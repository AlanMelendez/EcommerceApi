using AutoMapper;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Products;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
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

        return Result<PagedResult<ProductResponse>>.Success(response);
    }
}