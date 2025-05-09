using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using ReSharperPlugin.AlphabetizeIt.Actions;

namespace ReSharperPlugin.AlphabetizeIt;

[ContextAction(Description = "AlphabetizeIt", GroupType = typeof(CSharpContextActions), Name = "AlphabetizeIt", Priority = 1)]
public sealed class SortPropertiesContextAction : IContextAction
{
    private readonly ICSharpContextActionDataProvider _dataProvider;

    public SortPropertiesContextAction(ICSharpContextActionDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public IEnumerable<IntentionAction> CreateBulbItems()
    {
        // Create a bulb action with text that will appear in the menu
        List<IBulbAction> bulbItems = [];
        if (_dataProvider.GetSelectedElement<IClassDeclaration>() != null)
        {
            bulbItems.Add(new SortClassPropertiesAction(_dataProvider));
        }

        if (_dataProvider.GetSelectedElement<IObjectInitializer>() != null)
        {
            bulbItems.Add(new SortObjectInitializerPropertiesAction(_dataProvider));
        }

        return bulbItems.ToContextActionIntentions();
    }

    public bool IsAvailable(IUserDataHolder cache)
    {
        return _dataProvider.GetSelectedElement<IClassDeclaration>() != null
            || _dataProvider.GetSelectedElement<IObjectInitializer>() != null;
    }
}