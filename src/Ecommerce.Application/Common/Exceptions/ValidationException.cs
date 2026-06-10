using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Common.Exceptions;

public sealed class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToArray();
    }

    public IReadOnlyCollection<ValidationError> Errors { get; }
}