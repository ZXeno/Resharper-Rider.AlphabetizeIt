// using System.Collections.Generic;
// using JetBrains.ReSharper.Feature.Services.Bulbs;
// using JetBrains.ReSharper.Feature.Services.Intentions;
// using JetBrains.ReSharper.Feature.Services.QuickFixes;
// using JetBrains.ReSharper.Psi.CSharp.Tree;
// using JetBrains.Util;
// using ReSharperPlugin.AlphabetizeIt.Highlightings;
//
// namespace ReSharperPlugin.AlphabetizeIt;
//
// [QuickFix]
// public sealed class SortPropertiesQuickFix : IQuickFix
// {
//     private readonly IClassDeclaration _classDeclaration;
//
//     public SortPropertiesQuickFix(){}
//
//     // public SortPropertiesQuickFix(AbitHighlighter abitHighlighter)
//     // {
//     //     _classDeclaration = abitHighlighter.ClassDeclaration;
//     // }
//
//     public IEnumerable<IntentionAction> CreateBulbItems()
//     {
//         // Create a bulb action with text that will appear in the menu
//         List<IBulbAction> bulbItems = [new SortPropertiesAction(_classDeclaration)];
//
//         return bulbItems.ToQuickFixIntentions();
//     }
//
//     /// <summary>
//     /// Check if the class declaration is valid and has more than one property to sort.
//     /// </summary>
//     /// <param name="cache"></param>
//     /// <returns></returns>
//     public bool IsAvailable(IUserDataHolder cache)
//     {
//         return _classDeclaration.IsValid()
//                && _classDeclaration.PropertyDeclarations.Count > 1;
//     }
// }