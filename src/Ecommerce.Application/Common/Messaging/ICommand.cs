using Ecommerce.Application.Common.Models;
using MediatR;

namespace Ecommerce.Application.Common.Messaging;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;