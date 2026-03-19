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

public sealed class SortInterfacePropertiesAction : AbitActionBase
{
    private readonly IInterfaceDeclaration _interfaceDeclaration;
    private readonly ICSharpContextActionDataProvider _contextProvider;

    public SortInterfacePropertiesAction(ICSharpContextActionDataProvider provider)
    {
        _interfaceDeclaration = provider.GetSelectedElement<IInterfaceDeclaration>();
        _contextProvider = provider;
    }

    public override string Text => "Sort interface properties alphabetically";

    protected override void ExecuteAction(ISolution solution, ITextControl textControl)
    {
        IList<IPropertyDeclaration> properties =
            _interfaceDeclaration.MemberDeclarations
                .OfType<IPropertyDeclaration>()
                .OrderBy(p => p.DeclaredName)
                .ToList();

        List<CsharpRegion> regions = _contextProvider.PsiFile.GetAllRegions();

        if (properties.Count <= 1)
        {
            return;
        }

        bool propInRegion = false;
        Dictionary<string, IList<IPropertyDeclaration>> sortedProps = AbitHelper.CreateAccessorSorted<IPropertyDeclaration>();
        foreach (IPropertyDeclaration property in properties)
        {
            string accessorKey = string.Join(" ", property.ModifiersList?.ModifiersEnumerable.Select(x => x.NodeType) ?? []);

            // check if the property is contained in a region. If it is,
            // store that in the region's sorted props
            foreach (CsharpRegion region in regions)
            {
                if (region.ContainsOffset(property.GetTreeStartOffset()))
                {
                    region.TryAddSortedProperty(accessorKey, property);
                    propInRegion = true;
                    break;
                }
            }

            if (propInRegion)
            {
                propInRegion = false;
                continue;
            }

            if (!sortedProps.TryAdd(accessorKey, new List<IPropertyDeclaration> { property }))
            {
                sortedProps[accessorKey].Add(property);
            }
        }

        // Get the class body where properties are located
        IClassBody classBody = _interfaceDeclaration.Body;
        if (classBody == null)
        {
            return;
        }

        using WriteLockCookie cookie = WriteLockCookie.Create();
        CSharpElementFactory factory = CSharpElementFactory.GetInstance(_interfaceDeclaration);

        // Remove all properties
        foreach (IPropertyDeclaration prop in properties)
        {
            _interfaceDeclaration.RemoveClassMemberDeclaration(prop);
        }

        // Add the properties back in the sorted order after the last constructor and before methods.
        bool hasConstructors = _interfaceDeclaration.ConstructorDeclarations.Count > 0;
        bool hasMethods = _interfaceDeclaration.MethodDeclarations.Count > 0;
        ITreeNode anchor = hasConstructors && !regions.ContainsNode(_interfaceDeclaration.ConstructorDeclarations.Last())
            ? _interfaceDeclaration.ConstructorDeclarations.Last()
            : classBody;

        anchor = !hasConstructors && hasMethods && !regions.ContainsNode(_interfaceDeclaration.MethodDeclarations[0])
            ? _interfaceDeclaration.MethodDeclarations[0]
            : anchor;

        // set the region start anchor. We'll place properties above the first region
        // following a constructor in this scenario. Otherwise, we'll leave them alone.
        bool isRegionStartAnchor = false;
        if (regions.Count > 0)
        {
            if (!hasConstructors && !hasMethods)
            {
                isRegionStartAnchor = true;
                anchor = regions[0].Start;
            }
        }

        bool isPropAnchor = false;
        bool anchorIsClassBody = anchor is IClassBody;
        // Nested loops are ugly, but we have a fixed number of accessors. This
        // should be fine perf-wise for the expected size of the `props` object.
        foreach (string accessor in sortedProps.Keys)
        {
            foreach (IPropertyDeclaration prop in sortedProps[accessor])
            {
                IPropertyDeclaration newprop =
                    (IPropertyDeclaration)factory.CreateTypeMemberDeclaration(prop.GetText());

                IPropertyDeclaration addedProp;
                if (anchorIsClassBody)
                {
                    anchorIsClassBody = false;
                    if (regions.Count > 0)
                    {
                        anchor = regions[0].Start;
                        addedProp = addedProp = ModificationUtil.AddChildBefore(anchor, newprop);
                    }
                    else
                    {
                        addedProp = _interfaceDeclaration.AddClassMemberDeclaration(newprop);
                    }

                    anchor = addedProp;
                    isPropAnchor = true;
                    continue;
                }

                if (isPropAnchor || hasConstructors)
                {
                    addedProp = ModificationUtil.AddChildAfter(anchor, newprop);
                    anchor = addedProp;
                    isPropAnchor = true;
                    continue;
                }

                if (isRegionStartAnchor || hasMethods)
                {
                    addedProp = ModificationUtil.AddChildBefore(anchor, newprop);
                    anchor = addedProp;
                    isPropAnchor = true;
                    continue;
                }

                addedProp = _interfaceDeclaration.AddClassMemberDeclaration(newprop);
                anchor = addedProp;
                isPropAnchor = true;
            }
        }

        // add sorted region props
        // stars in the f*king sky, this is so many nested loops.
        foreach (CsharpRegion region in regions)
        {
            anchor = region.Start;
            foreach (string accessor in region.SortedProperties.Keys)
            {
                foreach (IPropertyDeclaration prop in region.SortedProperties[accessor])
                {
                    IPropertyDeclaration newprop =
                        (IPropertyDeclaration)factory.CreateTypeMemberDeclaration(prop.GetText());

                    IPropertyDeclaration addedProp = ModificationUtil.AddChildAfter(anchor, newprop);
                    anchor = addedProp;
                }
            }
        }

        classBody.FormatNode();
    }
}