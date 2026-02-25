using System.Collections.Generic;

namespace ReSharperPlugin.AlphabetizeIt.Utils;

public static class AbitHelper
{
    /// <summary>
    /// Create accessor sorting, we'll use the `NodeType` ToString() values for keys
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Dictionary<string, IList<T>> CreateAccessorSorted<T>()
    {
        return new Dictionary<string, IList<T>>
        {
            { "PUBLIC_KEYWORD", new List<T>() },
            { "INTERNAL_KEYWORD", new List<T>() },
            { "PROTECTED_KEYWORD INTERNAL_KEYWORD", new List<T>() },
            { "PROTECTED_KEYWORD", new List<T>() },
            { "PRIVATE_KEYWORD", new List<T>() },
        };
    }
}