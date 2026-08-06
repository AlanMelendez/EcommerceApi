using Ecommerce.Api.Authorization;
using Ecommerce.Api.Extensions;
using Ecommerce.Application.Features.Products.Commands.CreateProduct;
using Ecommerce.Application.Features.Products.Commands.DeleteProduct;
using Ecommerce.Application.Features.Products.Commands.UpdateProduct;
using Ecommerce.Application.Features.Products.Queries.GetProductById;
using Ecommerce.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string? search = null,
     [FromQuery] Guid? categoryId = null,
     [FromQuery] decimal? minPrice = null,
     [FromQuery] decimal? maxPrice = null,
     [FromQuery] string? sortBy = "name",
     [FromQuery] string? sortDirection = "asc",
     CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(
            pageNumber,
            pageSize,
            search,
            categoryId,
            minPrice,
            maxPrice,
            sortBy,
            sortDirection);

        var result = await _sender.Send(query, cancellationToken);

        return result.ToActionResult(this, "Products retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost]
    //[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }

    public sealed record UpdateProductRequest(
        string Name,
        string? Description,
        decimal Price,
        int Stock,
        Guid CategoryId);

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.CategoryId);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductCommand(id), cancellationToken);

        return result.ToActionResult(this);
    }
}
