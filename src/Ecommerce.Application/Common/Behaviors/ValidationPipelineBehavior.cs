using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Common.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .Select(error => new ValidationError(
                error.PropertyName,
                error.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length > 0)
        {
            throw new Exceptions.ValidationException(errors);
        }

        return await next(cancellationToken);
    }
}