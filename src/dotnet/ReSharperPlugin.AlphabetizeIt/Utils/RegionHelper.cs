using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharperPlugin.AlphabetizeIt.Utils;

public static class RegionHelper
{
    private const string RegionKeyword = "#region";

    public static string GetRegionName(this IPreprocessorDirective regionDirective)
    {
        // The region name is the text after "#region"
        string text = regionDirective.GetText();
        return text.StartsLike(RegionKeyword)
            ? text.Substring(RegionKeyword.Length).Trim()
            : string.Empty;
    }
}