namespace frontend.Utils;

/// <summary>
/// Utility functions for formatting
/// </summary>
public static class FormatUtils
{
    /// <summary>
    /// Formats a decimal value as currency (USD)
    /// </summary>
    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
    }

    /// <summary>
    /// Formats a decimal rating to 1 decimal place
    /// </summary>
    public static string FormatRating(decimal rating)
    {
        return rating.ToString("0.0");
    }

    /// <summary>
    /// Truncates a string to a specified length
    /// </summary>
    public static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength) + "...";
    }
}
