using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken);

    void Update(RefreshToken refreshToken);
}