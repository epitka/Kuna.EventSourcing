using System;
using Kuna.Extensions.DependencyInjection.Validation.Model;

namespace Carts.Tests.Exceptions;

public class FailureException : Exception
{
    public FailureException(Result result)
    {
        this.Result = result;
    }

    public Result Result { get; }
}