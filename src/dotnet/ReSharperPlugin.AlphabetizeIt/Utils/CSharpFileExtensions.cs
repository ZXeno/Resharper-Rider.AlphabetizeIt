using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperPlugin.AlphabetizeIt.Models;
using System.Collections.Generic;

namespace ReSharperPlugin.AlphabetizeIt.Utils;

public static class CSharpFileExtensions
{
    public static List<CsharpRegion> GetAllRegions(this ICSharpFile file)
    {
        List<CsharpRegion> regions = new();
        Stack<IPreprocessorDirective> regionStack = new();

        // Traverse all preprocessor directives in the file
        foreach (IPreprocessorDirective node in file.Descendants<IPreprocessorDirective>())
        {
            if (node.Kind == PreprocessorDirectiveKind.REGION)
            {
                regionStack.Push(node);
                continue;
            }

            if (node.Kind == PreprocessorDirectiveKind.ENDREGION && regionStack.Count > 0)
            {
                IPreprocessorDirective regionStart = regionStack.Pop();
                regions.Add(new CsharpRegion
                {
                    Start = regionStart,
                    End = node,
                    File = file,
                    Name = regionStart.GetRegionName(),
                });
            }
        }

        return regions;
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

        if (regionStart is null)
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
                File = file,
                Start = regionStart,
                Name = regionStart.GetRegionName(),
                End = regionEnd,
            };
    }
}