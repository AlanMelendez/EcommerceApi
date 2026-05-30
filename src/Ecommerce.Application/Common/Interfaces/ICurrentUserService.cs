using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }

        string? Email { get; }

        bool IsAuthenticated { get; }

        bool IsInRole(string role);
    }
}
