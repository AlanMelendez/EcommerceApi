using Ecommerce.Api.Authorization;
using Ecommerce.Api.Factories;
using Ecommerce.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccessController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public AccessController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult Authenticated()
    {
        var data = new
        {
            UserId = _currentUserService.UserId,
            Email = _currentUserService.Email,
            IsAuthenticated = _currentUserService.IsAuthenticated
        };

        return Ok(
            ApiResponseFactory.Success(
                data,
                "Authenticated access granted."));
    }

    [Authorize(Policy = AuthorizationPolicies.CustomerOnly)]
    [HttpGet("customer")]
    public IActionResult CustomerOnly()
    {
        var data = new
        {
            UserId = _currentUserService.UserId,
            Email = _currentUserService.Email,
            Role = "Customer",
            HasCustomerRole = _currentUserService.IsInRole("Customer")
        };

        return Ok(
            ApiResponseFactory.Success(
                data,
                "Customer access granted."));
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        var data = new
        {
            UserId = _currentUserService.UserId,
            Email = _currentUserService.Email,
            Role = "Admin",
            HasAdminRole = _currentUserService.IsInRole("Admin")
        };

        return Ok(
            ApiResponseFactory.Success(
                data,
                "Administrator access granted."));
    }
}