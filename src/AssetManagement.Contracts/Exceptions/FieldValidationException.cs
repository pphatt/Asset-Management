namespace AssetManagement.Contracts.Exceptions;

public class FieldValidationException : Exception
{
    public string Field { get; }

    public FieldValidationException(string field, string message)
        : base(message)
    {
        Field = field;
    }
}
