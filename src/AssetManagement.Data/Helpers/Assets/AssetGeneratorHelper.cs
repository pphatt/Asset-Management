using AssetManagement.Domain.Entities;

namespace AssetManagement.Data.Helpers.Assets;

public static class AssetGeneratorHelper
{
    public static string GenerateAssetCode(string prefix, Asset? asset)
    {
        if (asset is null)
        {
            return $"{prefix}000001";
        }
        else
        {
            // Extract the number part from the last asset code
            string numberPart = asset.Code.Substring(prefix.Length);
    
            // Parse the number and increment it
            if (int.TryParse(numberPart, out int lastNumber))
            {
                int nextNumber = lastNumber + 1;
                // Format with 6 digits, padding with zeros
                return $"{prefix}{nextNumber:D6}";
            }
            return $"{prefix}000001";
    
            // Fallback if parsing fails
        }
    }
}