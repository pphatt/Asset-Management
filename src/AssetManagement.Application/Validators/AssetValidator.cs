using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Domain.Entities;

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

    public static void ValidateUpdateAsset(UpdateAssetRequestDto dto, Asset existingAsset)
    {
        var errors = new List<FieldValidationException>();

        if (dto.State.HasValue)
            AddErrorIfInvalidEnum(errors, dto.State.Value, "State", "Invalid asset state value");

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
        if (Convert.ToInt32(value) == 0 || !Enum.IsDefined(typeof(T), value))
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