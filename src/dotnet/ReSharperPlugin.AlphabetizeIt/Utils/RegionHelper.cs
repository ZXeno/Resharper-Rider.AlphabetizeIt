using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperPlugin.AlphabetizeIt.Models;
using System.Collections.Generic;

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

    public static bool ContainsNode(this List<CsharpRegion> regions, ITreeNode node)
    {
        foreach (CsharpRegion region in regions)
        {
            if (region.ContainsOffset(node.GetTreeStartOffset()))
            {
                return true;
            }
        }

        return false;
    }

    public static CsharpRegion FindRegionAtOffset(ICSharpFile file, DocumentOffset offset)
    {
        TreeOffset treeOffset = file.Translate(offset);
        ITreeNode token = file.FindTokenAt(treeOffset);
        if (token == null)
        {
            return null;
        }

        IPreprocessorDirective regionStart = null;
        IPreprocessorDirective regionEnd = null;
        int nestingLevel = 0;

        // Find region opener
        ITreeNode currentNode = token.GetContainingNode<ITreeNode>(true);
        while (currentNode != null)
        {
            if (currentNode is IPreprocessorDirective directive)
            {
                if (directive.Kind == PreprocessorDirectiveKind.REGION)
                {
                    if (nestingLevel == 0)
                    {
                        regionStart = directive;
                        break;
                    }
                    nestingLevel--;
                }
                else if (directive.Kind == PreprocessorDirectiveKind.ENDREGION)
                {
                    nestingLevel++;
                }
            }

            currentNode = currentNode.PrevSibling ?? currentNode.Parent;
        }

        if (regionStart == null)
        {
            return null;
        }

        // Find endregion directive
        nestingLevel = 0;
        currentNode = regionStart.NextSibling;
        while (currentNode != null)
        {
            if (currentNode is IPreprocessorDirective directive)
            {
                if (directive.Kind == PreprocessorDirectiveKind.REGION)
                {
                    nestingLevel++;
                }
                else if (directive.Kind == PreprocessorDirectiveKind.ENDREGION)
                {
                    if (nestingLevel == 0)
                    {
                        regionEnd = directive;
                        break;
                    }
                    nestingLevel--;
                }
            }
            currentNode = currentNode.NextSibling ?? currentNode.Parent?.NextSibling;
        }

        if (regionEnd is null)
        {
            return null;
        }

        return new CsharpRegion
            {
                End = regionEnd,
                File = file,
                Name = regionStart.GetRegionName(),
                Start = regionStart,
            };
    }
}