using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperPlugin.AlphabetizeIt.Utils;
using System.Collections.Generic;

namespace ReSharperPlugin.AlphabetizeIt.Models;

public sealed class CsharpRegion
{
    public CsharpRegion()
    {
        SortedProperties = AbitHelper.CreateAccessorSorted<IPropertyDeclaration>();
        SortedMethods = AbitHelper.CreateAccessorSorted<IMethodDeclaration>();
    }

    public string Name { get; init; }
    public ICSharpFile File { get; init; }
    public IPreprocessorDirective Start { get; init; }
    public TreeOffset StartOffset => Start.GetTreeStartOffset();
    public DocumentOffset StartDocOffset => Start.GetDocumentStartOffset();
    public IPreprocessorDirective End { get; init; }
    public DocumentOffset EndDocOffset => End.GetDocumentEndOffset();
    public TreeOffset EndOffset => End.GetTreeEndOffset();
    public Dictionary<string, IList<IPropertyDeclaration>> SortedProperties { get; private set; }
    public Dictionary<string, IList<IMethodDeclaration>> SortedMethods { get; set; }
    public bool HasProperties => SortedProperties.Count > 0 || this.GetPropertiesInRegion().Count > 0;
    public bool HasMethods => SortedMethods.Count > 0 || this.GetMethodsInRegion().Count > 0;

    public TreeTextRange Range => new TreeTextRange(StartOffset, EndOffset);

    public void TryAddSortedProperty(string accessorKey, IPropertyDeclaration prop)
    {
        if (!SortedProperties.TryAdd(accessorKey, new List<IPropertyDeclaration> { prop }))
        {
            SortedProperties[accessorKey].Add(prop);
        }
    }

    public void TryAddSortedMethod(string accessorKey, IMethodDeclaration prop)
    {
        if (!SortedMethods.TryAdd(accessorKey, new List<IMethodDeclaration> { prop }))
        {
            SortedMethods[accessorKey].Add(prop);
        }
    }

    public bool ContainsOffset(DocumentOffset docOffset)
    {
        if (!docOffset.IsValid())
        {
            return false;
        }

        return docOffset > StartDocOffset && docOffset < EndDocOffset;
    }

    public bool ContainsOffset(TreeOffset treeOffset)
    {
        if (!treeOffset.IsValid())
        {
            return false;
        }

        return treeOffset > StartOffset && treeOffset < EndOffset;
    }

    public List<IPropertyDeclaration> GetPropertiesInRegion()
    {
        List<IPropertyDeclaration> properties = new();
        TreeOffset startOffset = this.Start.GetTreeEndOffset();
        TreeOffset endOffset = End.GetTreeStartOffset();

        // Get the class or type declaration that contains the region
        IClassLikeDeclaration typeDeclaration = this.Start.GetContainingNode<IClassLikeDeclaration>();
        if (typeDeclaration == null)
        {
            return properties;
        }

        foreach (ICSharpTypeMemberDeclaration member in typeDeclaration.MemberDeclarations)
        {
            if (member is not IPropertyDeclaration property)
            {
                continue;
            }

            TreeOffset propertyOffset = property.GetTreeStartOffset();
            if (propertyOffset >= startOffset && propertyOffset <= endOffset)
            {
                properties.Add(property);
            }
        }

        return properties;
    }

    public List<IMethodDeclaration> GetMethodsInRegion()
    {
        List<IMethodDeclaration> methods = new();
        TreeOffset startOffset = this.Start.GetTreeEndOffset();
        TreeOffset endOffset = End.GetTreeStartOffset();

        // Get the class or type declaration that contains the region
        IClassLikeDeclaration typeDeclaration = this.Start.GetContainingNode<IClassLikeDeclaration>();
        if (typeDeclaration == null)
        {
            return methods;
        }

        foreach (ICSharpTypeMemberDeclaration member in typeDeclaration.MemberDeclarations)
        {
            if (member is not IMethodDeclaration method)
            {
                continue;
            }

            TreeOffset methodOffset = method.GetTreeStartOffset();
            if (methodOffset >= startOffset && methodOffset <= endOffset)
            {
                methods.Add(method);
            }
        }

        return methods;
    }

    public bool ContainsNode(ITreeNode node)
    {
        return node.GetDocumentStartOffset() > this.StartDocOffset
               && node.GetDocumentEndOffset() < this.EndDocOffset;
    }
}