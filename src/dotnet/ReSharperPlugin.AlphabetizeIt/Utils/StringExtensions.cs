using System;

namespace ReSharperPlugin.AlphabetizeIt.Utils;

public static class StringExtensions
{
    public static bool StartsLike(this string? str, object? other)
    {
        if ((str is null && other is not null)
            || str is not null && other is null)
        {
            return false;
        }

        if (str is null && other is null)
        {
            return true;
        }

        if (other is string otherStr)
        {

            return str.StartsWith(otherStr, StringComparison.OrdinalIgnoreCase);
        }

        return str.StartsWith(other?.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}