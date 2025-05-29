using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;

namespace AssetManagement.Application.Validators;

public static class AssetValidator
{
    public static void ValidateAsset(CreateAssetRequestDto asset)
    {
        var error = new List<FieldValidationException>();
        
        AddErrorIfEmpty(error, asset.Name, "Name", "Name is required");
        AddErrorIfEmpty(error, asset.Specifications, "Specifications", "Specifications is required");
        AddErrorIfEmpty(error, asset.CategoryId.ToString(), "Category", "Category is required");
        AddErrorIfEmpty(error, asset.InstalledDate, "Installed Date", "Installed Date is required");
        AddErrorIfInvalidEnum(error, asset.State, "State", "Invalid state");
        
    }
    
    private static void AddErrorIfEmpty(List<FieldValidationException> errors, string? value, string field, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(new FieldValidationException(field, message));
    }
    private static void AddErrorIfInvalidEnum<T>(List<FieldValidationException> errors, T value, string field, string message) where T : Enum
    {
        if (Convert.ToInt32(value) == 0 || !Enum.IsDefined(typeof(T), value))
            errors.Add(new FieldValidationException(field, message));
    }
}