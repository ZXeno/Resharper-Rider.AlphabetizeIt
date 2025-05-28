using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Application.Threading;
using JetBrains.DocumentModel;
using JetBrains.DocumentModel.Storage;
using JetBrains.DocumentModel.Transactions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;


namespace ReSharperPlugin.AlphabetizeIt.Actions;

public sealed class SortClassPropertiesAction : AbitActionBase
{
    private readonly IClassDeclaration _classDeclaration;

    public SortClassPropertiesAction(ICSharpContextActionDataProvider provider)
    {
        _classDeclaration = provider.GetSelectedElement<IClassDeclaration>();
    }

    public override string Text => "Sort class properties alphabetically";

    protected override void ExecuteAction(ISolution solution, ITextControl textControl)
    {
        IList<IPropertyDeclaration> properties =
            _classDeclaration.MemberDeclarations
                .OfType<IPropertyDeclaration>()
                .OrderBy(p => p.DeclaredName)
                .ToList();

        if (properties.Count <= 1)
        {
            return;
        }

        // create accessor sorting, we'll use the `NodeType` ToString() values for keys
        Dictionary<string, IList<IPropertyDeclaration>> sortedProps = new()
        {
            {"PUBLIC_KEYWORD", new List<IPropertyDeclaration>()},
            {"INTERNAL_KEYWORD", new List<IPropertyDeclaration>()},
            {"PROTECTED_KEYWORD INTERNAL_KEYWORD", new List<IPropertyDeclaration>()},
            {"PROTECTED_KEYWORD", new List<IPropertyDeclaration>()},
            {"PRIVATE_KEYWORD", new List<IPropertyDeclaration>()},
        };

        foreach (IPropertyDeclaration property in properties)
        {
            string accessorKey = string.Join(" ", property.ModifiersList.ModifiersEnumerable.Select(x => x.NodeType));
            if (!sortedProps.TryAdd(accessorKey, new List<IPropertyDeclaration> { property }))
            {
                sortedProps[accessorKey].Add(property);
            }
        }

        // Get the class body where properties are located
        IClassBody classBody = _classDeclaration.Body;
        if (classBody == null)
        {
            return;
        }

        using WriteLockCookie cookie = WriteLockCookie.Create();
        CSharpElementFactory factory = CSharpElementFactory.GetInstance(_classDeclaration);

        // Remove all properties
        foreach (IPropertyDeclaration prop in properties)
        {
            _classDeclaration.RemoveClassMemberDeclaration(prop);
        }

        // Add the properties back in the sorted order after the last constructor and before methods.
        bool hasConstructors = _classDeclaration.ConstructorDeclarations.Count > 0;
        bool hasMethods = _classDeclaration.MethodDeclarations.Count > 0;
        ITreeNode anchor = hasConstructors
            ? _classDeclaration.ConstructorDeclarations.Last()
            : classBody;

        anchor = !hasConstructors && hasMethods
            ? _classDeclaration.MethodDeclarations[0]
            : anchor;

        bool isPropAnchor = false;

        // Nested loops are ugly, but we have a fixed number of accessors. This
        // should be fine perf-wise for the expected size of the `props` object.
        foreach (string accessor in sortedProps.Keys)
        {
            foreach (IPropertyDeclaration prop in sortedProps[accessor])
            {
                IPropertyDeclaration newprop =
                    (IPropertyDeclaration)factory.CreateTypeMemberDeclaration(prop.GetText());

                if (isPropAnchor || hasConstructors)
                {
                    IPropertyDeclaration addedProp = ModificationUtil.AddChildAfter(anchor, newprop);
                    anchor = addedProp;
                    isPropAnchor = true;
                    continue;
                }

                if (hasMethods)
                {
                    IPropertyDeclaration addedProp = ModificationUtil.AddChildBefore(anchor, newprop);
                    anchor = addedProp;
                    isPropAnchor = true;
                    continue;
                }

                _classDeclaration.AddClassMemberDeclaration(newprop);
            }
        }

        classBody.FormatNode();
    }
}