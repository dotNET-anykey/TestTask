namespace partycli.Extensions;

public static class StringExtensions
{
    public static bool EqualsToParameter(this string? value, string parameter) =>
        !string.IsNullOrEmpty(value) && value.Equals(parameter, StringComparison.OrdinalIgnoreCase);
}