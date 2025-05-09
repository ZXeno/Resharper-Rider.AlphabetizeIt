// using JetBrains.Annotations;
// using JetBrains.DocumentModel;
// using JetBrains.ReSharper.Feature.Services.Daemon;
// using JetBrains.ReSharper.Psi.CSharp;
// using JetBrains.ReSharper.Psi.CSharp.Tree;
// using JetBrains.ReSharper.Psi.Tree;
// using JetBrains.TextControl.DocumentMarkup;
// using JetBrains.UI.RichText;
//
// namespace ReSharperPlugin.AlphabetizeIt.Highlightings;
//
// [UsedImplicitly(
//     ImplicitUseKindFlags.Access,
//     ImplicitUseTargetFlags.WithMembers | ImplicitUseTargetFlags.WithInheritors)]
// public abstract class AbitHighlightingBase : IRichTextToolTipHighlighting
// {
//     protected readonly ITreeNode Element;
//     protected readonly string Format;
//     protected readonly RichText Description;
//
//     protected AbitHighlightingBase(ITreeNode element, string format, RichText description)
//     {
//         Element = element;
//         Format = format;
//         Description = description;
//     }
//
//     public virtual bool IsValid() => Element.IsValid();
//
//     public DocumentRange CalculateRange()
//     {
//         if (Element is ICSharpExpression expression)
//         {
//             return expression.GetDocumentRange();
//         }
//
//         return Element.GetDocumentRange();
//     }
//
//     public RichText RichToolTip => RichText.Format(Format, Description);
//     public string ToolTip => RichToolTip.Text;
//     public string ErrorStripeToolTip => ToolTip;
//
//     public RichTextBlock? TryGetTooltip(HighlighterTooltipKind where)
//     {
//         return HighlightingToolTipHelper.CreateRichTextBlock(RichToolTip, CSharpLanguage.Instance!, where);
//     }
// }