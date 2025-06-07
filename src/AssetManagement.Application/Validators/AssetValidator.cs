using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;

namespace AssetManagement.Application.Validators;

public static class AssetValidator
{
    public static void ValidateAsset(CreateAssetRequestDto asset)
    {
        var errors = new List<FieldValidationException>();

        AddErrorIfEmpty(errors, asset.Name, "Name", "Name is required");
        AddErrorIfEmpty(errors, asset.Specifications, "Specifications", "Specifications is required");
        AddErrorIfEmpty(errors, asset.CategoryId.ToString(), "Category", "Category is required");
        AddErrorIfEmpty(errors, asset.InstalledDate, "Installed Date", "Installed Date is required");
        AddErrorIfInvalidEnum(errors, asset.State, "State", "Invalid state");

        ThrowIfErrors(errors);
    }

    public static void ValidateUpdateAsset(UpdateAssetRequestDto dto)
    {
        var errors = new List<FieldValidationException>();

        AddErrorIfEmpty(errors, dto.Name, "Name", "Asset name is required");
        if (dto.State is not null)
        {
            AddErrorIfInvalidEnum(errors, dto.State.Value, "State", "Invalid state value");
        }
        DateTimeOffset? installedDate = ValidateInstalledDate(errors, dto.InstalledDate);

        ValidateInstalledDateNotInFuture(errors, installedDate);

        ThrowIfErrors(errors);
    }

    private static void AddErrorIfEmpty(List<FieldValidationException> errors, string? value, string field, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(new FieldValidationException(field, message));
    }

    private static void AddErrorIfInvalidEnum<T>(List<FieldValidationException> errors, T value, string field, string message) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
            errors.Add(new FieldValidationException(field, message));
    }

    private static void ThrowIfErrors(List<FieldValidationException> errors)
    {
        if (errors.Count > 0)
        {
            throw new AggregateFieldValidationException(errors);
        }
    }

    private static DateTime? ValidateInstalledDate(List<FieldValidationException> errors, string? installedDate)
    {
        if (string.IsNullOrWhiteSpace(installedDate))
        {
            return null;
        }

        if (!DateTime.TryParse(installedDate, out var date))
        {
            errors.Add(new FieldValidationException("InstalledDate", "Invalid installed date format"));
            return null;
        }

        return date;
    }

    private static void ValidateInstalledDateNotInFuture(List<FieldValidationException> errors, DateTimeOffset? installedDate)
    {
        if (installedDate.HasValue && installedDate.Value > DateTimeOffset.UtcNow)
        {
            errors.Add(new FieldValidationException("InstalledDate", "Installed date cannot be in the future"));
        }
    }
}