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
        IObjectInitializer objInitializer = _dataProvider.GetSelectedElement<IObjectInitializer>();
        IClassDeclaration classDec = _dataProvider.GetSelectedElement<IClassDeclaration>();
        bool isObjInitializer = objInitializer != null;
        bool isClass = classDec != null;
        List<IBulbAction> bulbItems = [];

        if (isObjInitializer && objInitializer.MemberInitializers.Count > 1)
        {
            bulbItems.Add(new SortObjectInitializerPropertiesAction(_dataProvider));
        }

        if (!isObjInitializer && isClass && classDec.PropertyDeclarations.Count > 1)
        {
            bulbItems.Add(new SortClassPropertiesAction(_dataProvider));
        }

        return bulbItems.ToContextActionIntentions();
    }

    public bool IsAvailable(IUserDataHolder cache)
    {
        IObjectInitializer objInitializer = _dataProvider.GetSelectedElement<IObjectInitializer>();
        IClassDeclaration classDec = _dataProvider.GetSelectedElement<IClassDeclaration>();

        return (classDec != null && classDec.PropertyDeclarations.Count > 1)
               || (objInitializer != null && objInitializer.MemberInitializers.Count > 1);
    }
}