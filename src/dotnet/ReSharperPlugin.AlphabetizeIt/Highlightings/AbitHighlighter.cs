// using JetBrains.Annotations;
// using JetBrains.DocumentModel;
// using JetBrains.ReSharper.Feature.Services.Daemon;
// using JetBrains.ReSharper.Psi.CSharp.Tree;
// using JetBrains.ReSharper.Psi.Tree;
// using JetBrains.TextControl.DocumentMarkup;
// using JetBrains.UI.RichText;
//
// namespace ReSharperPlugin.AlphabetizeIt.Highlightings;
//
// [RegisterConfigurableSeverity(
//     ID: SeverityId,
//     CompoundItemName: null,
//     Group: AbitHighlightingsGroupIds.Id,
//     Title: "Sort Properties",
//     Description: "Highlights classes where properties can be sorted",
//     DefaultSeverity: Severity.HINT)]
// public sealed class AbitHighlighter : AbitHighlightingBase
// {
//     public const string SeverityId = "AlphabetizeIt.Sort";
//     public const string Message = "Sort properties of: {0}";
//
//     public AbitHighlighter(ITreeNode element)
//         : base(element, "{0}", new RichText("Sort properties alphabetically"))
//     {
//         if (element is IClassDeclaration classDeclaration)
//         {
//             ClassDeclaration = classDeclaration;
//         }
//     }
//
//     [CanBeNull] public IClassDeclaration ClassDeclaration { get; }
//
//
//     public DocumentRange CalculateRange()
//     {
//         return ClassDeclaration?.NameIdentifier.GetDocumentRange()
//                ?? Element.GetDocumentRange();
//     }
//
//     public RichTextBlock TryGetTooltip(HighlighterTooltipKind where)
//     {
//         return new RichTextBlock(new RichText(ToolTip));
//     }
// }