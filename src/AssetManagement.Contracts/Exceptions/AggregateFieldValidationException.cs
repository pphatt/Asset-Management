using System;
using System.Collections.Generic;

namespace AssetManagement.Contracts.Exceptions;

public class AggregateFieldValidationException : Exception
{
    public List<FieldValidationException> Errors { get; }

    public AggregateFieldValidationException(List<FieldValidationException> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}