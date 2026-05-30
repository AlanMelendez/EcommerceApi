using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Common.Models
{
    public sealed class Result<T> : Result
    {
        private readonly T? _value;

        private Result(T value)
            : base(true, Error.None)
        {
            _value = value;
        }

        private Result(Error error)
            : base(false, error)
        {
            _value = default;
        }

        public T Value
        {
            get
            {
                if (IsFailure)
                {
                    throw new InvalidOperationException("Cannot access value from a failed result.");
                }

                return _value!;
            }
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        public static new Result<T> Failure(Error error)
        {
            return new Result<T>(error);
        }
    }
}
