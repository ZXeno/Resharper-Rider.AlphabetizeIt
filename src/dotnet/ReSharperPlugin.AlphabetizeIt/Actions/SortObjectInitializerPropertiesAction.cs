using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.AlphabetizeIt.Actions;

public sealed class SortObjectInitializerPropertiesAction : AbitActionBase
{
    private readonly IObjectInitializer _objInitializer;

    public SortObjectInitializerPropertiesAction(ICSharpContextActionDataProvider contextProvider)
    {
        _objInitializer = contextProvider.GetSelectedElement<IObjectInitializer>();
    }

    public override string Text => "Sort object initializer properties alphabetically";

    protected override void ExecuteAction(ISolution solution, ITextControl textControl)
    {
        IMemberInitializer[] originalList =
            _objInitializer.MemberInitializers
                .ToArray();

        IPropertyInitializer[] initializers =
            _objInitializer.MemberInitializers
                .OfType<IPropertyInitializer>()
                .OrderBy(p => p.NameIdentifier.Name)
                .ToArray();

        if (initializers.Length <= 1)
        {
            return;
        }

        using WriteLockCookie cookie = WriteLockCookie.Create();

        for (int x = 0; x < originalList.Length; x++)
        {
            ModificationUtil.ReplaceChild(originalList[x], initializers[x]);
        }

        _objInitializer.FormatNode();
    }
}