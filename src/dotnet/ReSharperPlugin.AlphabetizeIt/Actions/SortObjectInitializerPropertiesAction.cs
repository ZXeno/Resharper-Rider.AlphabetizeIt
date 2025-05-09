using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.AlphabetizeIt.Actions;

public sealed class SortObjectInitializerPropertiesAction : AbitActionBase
{
    private readonly ICSharpContextActionDataProvider _contextProvider;
    private readonly IObjectInitializer _objInitializer;

    public SortObjectInitializerPropertiesAction(ICSharpContextActionDataProvider contextProvider)
    {
        _contextProvider = contextProvider;
        _objInitializer = _contextProvider.GetSelectedElement<IObjectInitializer>();
    }

    public override string Text => "Sort object initializer properties alphabetically";

    protected override void ExecuteAction(ISolution solution, ITextControl textControl)
    {
        List<IMemberInitializer> originalList = _objInitializer.MemberInitializers.ToList();

        List<IPropertyInitializer> initializers =
            _objInitializer.MemberInitializers
                .OfType<IPropertyInitializer>()
                .OrderBy(p => p.NameIdentifier.Name)
                .ToList();

        if (initializers.Count <= 1)
        {
            return;
        }

        using WriteLockCookie cookie = WriteLockCookie.Create();
        CSharpElementFactory factory = _contextProvider.ElementFactory;

        for (int x = 0; x < originalList.Count; x++)
        {
            ModificationUtil.ReplaceChild(originalList[x], initializers[x]);
        }

        _objInitializer.FormatNode();
    }
}