using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using ReSharperPlugin.AlphabetizeIt.Actions;
using System.Collections.Generic;

namespace ReSharperPlugin.AlphabetizeIt;

[ContextAction(Description = "AlphabetizeIt", GroupType = typeof(CSharpContextActions), Name = "AlphabetizeIt", Priority = 1)]
public sealed class SortContextAction : IContextAction
{
    private readonly ICSharpContextActionDataProvider _dataProvider;

    public SortContextAction(ICSharpContextActionDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public IEnumerable<IntentionAction> CreateBulbItems()
    {
        // Create a bulb action with text that will appear in the menu
        IObjectInitializer objInitializer = _dataProvider.GetSelectedElement<IObjectInitializer>();
        IClassDeclaration classDec = _dataProvider.GetSelectedElement<IClassDeclaration>();
        IInterfaceDeclaration iDec = _dataProvider.GetSelectedElement<IInterfaceDeclaration>();
        bool isObjInitializer = objInitializer != null;
        bool isClass = classDec != null;
        bool isInterface = iDec != null;
        List<IBulbAction> bulbItems = [];

        if (isObjInitializer && objInitializer.MemberInitializers.Count > 1)
        {
            bulbItems.Add(new SortObjectInitializerPropertiesAction(_dataProvider));
        }

        if (!isObjInitializer && isClass && classDec.PropertyDeclarations.Count > 1)
        {
            bulbItems.Add(new SortClassPropertiesAction(_dataProvider));
        }

        if (!isObjInitializer && isClass && classDec.MethodDeclarations.Count > 1)
        {
            bulbItems.Add(new SortClassMethodsAction(_dataProvider));
        }

        if (!isObjInitializer && isInterface && iDec.PropertyDeclarations.Count > 1)
        {
            bulbItems.Add(new SortInterfacePropertiesAction(_dataProvider));
        }

        if (!isObjInitializer && isInterface && iDec.MethodDeclarations.Count > 1)
        {
            bulbItems.Add(new SortInterfaceMethodsAction(_dataProvider));
        }

        return bulbItems.ToContextActionIntentions();
    }

    public bool IsAvailable(IUserDataHolder cache)
    {
        IObjectInitializer objInitializer = _dataProvider.GetSelectedElement<IObjectInitializer>();
        IClassDeclaration classDec = _dataProvider.GetSelectedElement<IClassDeclaration>();
        IInterfaceDeclaration iDec = _dataProvider.GetSelectedElement<IInterfaceDeclaration>();

        return (classDec != null && (classDec.PropertyDeclarations.Count > 1 || classDec.MethodDeclarations.Count > 1))
               || (iDec != null && (iDec.PropertyDeclarations.Count > 1 || iDec.MethodDeclarations.Count > 1))
               || (objInitializer != null && objInitializer.MemberInitializers.Count > 1);
    }
}