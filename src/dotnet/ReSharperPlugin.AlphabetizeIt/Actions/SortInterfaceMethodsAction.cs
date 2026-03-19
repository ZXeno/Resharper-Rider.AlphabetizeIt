using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using ReSharperPlugin.AlphabetizeIt.Models;
using ReSharperPlugin.AlphabetizeIt.Utils;
using System.Collections.Generic;
using System.Linq;

namespace ReSharperPlugin.AlphabetizeIt.Actions;

public sealed class SortInterfaceMethodsAction : AbitActionBase
{
    private readonly IInterfaceDeclaration _interfaceDeclaration;
    private readonly ICSharpContextActionDataProvider _contextProvider;

    public SortInterfaceMethodsAction(ICSharpContextActionDataProvider provider)
    {
        _interfaceDeclaration = provider.GetSelectedElement<IInterfaceDeclaration>();
        _contextProvider = provider;
    }

    public override string Text => "Sort interface methods alphabetically";

    protected override void ExecuteAction(ISolution solution, ITextControl textControl)
    {
        IList<IMethodDeclaration> methods =
            _interfaceDeclaration.MemberDeclarations
                .OfType<IMethodDeclaration>()
                .OrderBy(p => p.DeclaredName)
                .ToList();

        List<CsharpRegion> regions = _contextProvider.PsiFile.GetAllRegions();

        if (methods.Count <= 1)
        {
            return;
        }

        bool methodInRegion = false;
        Dictionary<string, IList<IMethodDeclaration>> sortedMethods = AbitHelper.CreateAccessorSorted<IMethodDeclaration>();
        foreach (IMethodDeclaration method in methods)
        {
            string accessorKey = string.Join(" ", method.ModifiersList?.ModifiersEnumerable.Select(x => x.NodeType) ?? []);

            // check if the property is contained in a region. If it is,
            // store that in the region's sorted props
            foreach (CsharpRegion region in regions)
            {
                if (region.ContainsOffset(method.GetTreeStartOffset()))
                {
                    region.TryAddSortedMethod(accessorKey, method);
                    methodInRegion = true;
                    break;
                }
            }

            if (methodInRegion)
            {
                methodInRegion = false;
                continue;
            }

            if (!sortedMethods.TryAdd(accessorKey, new List<IMethodDeclaration> { method }))
            {
                sortedMethods[accessorKey].Add(method);
            }
        }

        // Get the class body where methods are located
        IClassBody classBody = _interfaceDeclaration.Body;
        if (classBody == null)
        {
            return;
        }

        using WriteLockCookie cookie = WriteLockCookie.Create();
        CSharpElementFactory factory = CSharpElementFactory.GetInstance(_interfaceDeclaration);

        // Remove all methods
        foreach (IMethodDeclaration method in methods)
        {
            _interfaceDeclaration.RemoveClassMemberDeclaration(method);
        }

        // Add the properties back in the sorted order after the last constructor and before methods.
        bool hasConstructors = _interfaceDeclaration.ConstructorDeclarations.Count > 0;
        bool hasProperties = _interfaceDeclaration.PropertyDeclarations.Count > 0;
        ITreeNode anchor;
        if (hasProperties && !regions.ContainsNode(_interfaceDeclaration.PropertyDeclarations.Last()))
        {
            anchor = _interfaceDeclaration.PropertyDeclarations.Last();
        }
        else if (hasConstructors && !regions.ContainsNode(_interfaceDeclaration.PropertyDeclarations.Last()))
        {
            anchor = _interfaceDeclaration.ConstructorDeclarations.Last();
        }
        else
        {
            anchor = classBody;
        }

        // set the region start anchor. We'll place methods above the first region
        // following any constructors or properties in this scenario.
        bool isRegionStartAnchor = false;
        if (regions.Count > 0)
        {
            if (!hasConstructors && !hasProperties)
            {
                isRegionStartAnchor = true;
                anchor = regions.Last().End;
            }
        }

        bool isMethodAnchor = false;
        bool anchorIsClassBody = anchor is IClassBody;
        // Nested loops are ugly, but we have a fixed number of accessors. This
        // should be fine perf-wise for the expected size of the `props` object.
        foreach (string accessor in sortedMethods.Keys)
        {
            foreach (IMethodDeclaration prop in sortedMethods[accessor])
            {
                IMethodDeclaration newMethod =
                    (IMethodDeclaration)factory.CreateTypeMemberDeclaration(prop.GetText());

                IMethodDeclaration addedMethod;
                if (anchorIsClassBody)
                {
                    anchorIsClassBody = false;
                    if (regions.Count > 0)
                    {
                        anchor = regions[0].Start;
                        addedMethod = addedMethod = ModificationUtil.AddChildBefore(anchor, newMethod);
                    }
                    else
                    {
                        addedMethod = _interfaceDeclaration.AddClassMemberDeclaration(newMethod);
                    }

                    anchor = addedMethod;
                    isMethodAnchor = true;
                    continue;
                }

                if (isMethodAnchor && !isRegionStartAnchor)
                {
                    addedMethod = ModificationUtil.AddChildAfter(anchor, newMethod);
                    anchor = addedMethod;
                    isMethodAnchor = true;
                    continue;
                }

                if (isRegionStartAnchor)
                {
                    addedMethod = ModificationUtil.AddChildBefore(anchor, newMethod);
                    anchor = addedMethod;
                    isMethodAnchor = true;
                    continue;
                }

                addedMethod = _interfaceDeclaration.AddClassMemberDeclaration(newMethod);
                anchor = addedMethod;
                isMethodAnchor = true;
            }
        }

        // add sorted region props
        // stars in the f*king sky, this is so many nested loops.
        foreach (CsharpRegion region in regions)
        {
            anchor = region.Start;
            foreach (string accessor in region.SortedProperties.Keys)
            {
                foreach (IMethodDeclaration method in region.SortedMethods[accessor])
                {
                    IMethodDeclaration newMethod =
                        (IMethodDeclaration)factory.CreateTypeMemberDeclaration(method.GetText());

                    IMethodDeclaration addedProp = ModificationUtil.AddChildAfter(anchor, newMethod);
                    anchor = addedProp;
                }
            }
        }

        classBody.FormatNode();
    }
}