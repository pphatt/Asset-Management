namespace AssetManagement.Application.Extensions
{
    public static class EnumExtensions
    {
        public static T? GetEnum<T>(this string value, string? message) where T : struct
        {
            if (value == null)
            {
                return null;
            }

            if (int.TryParse(value, out int parsedInt) && Enum.IsDefined(typeof(T), parsedInt))
            {
                return (T)(object)parsedInt;
            }

            if (Enum.GetNames(typeof(T)).Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            throw new ArgumentException(message ?? $"Invalid value '{value}' for enum type '{typeof(T).Name}'.");
        }
    }
}
