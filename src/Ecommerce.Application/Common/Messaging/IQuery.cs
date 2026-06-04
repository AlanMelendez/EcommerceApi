using Ecommerce.Application.Common.Models;
using MediatR;

namespace Ecommerce.Application.Common.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}