using System;
using Kuna.Extensions.DependencyInjection.Validation.Model;

namespace Carts.Tests.Exceptions;

public class SuccessException : Exception
{
    public SuccessException(Result result)
    {
        this.Result = result;
    }

    public Result Result { get; }
}
