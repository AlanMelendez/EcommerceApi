using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Common.Interfaces;

public interface IPasswordHashingService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string password);
}