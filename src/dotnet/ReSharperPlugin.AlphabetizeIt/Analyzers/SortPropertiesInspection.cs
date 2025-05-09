// using JetBrains.Annotations;
// using JetBrains.ReSharper.Feature.Services.Daemon;
// using JetBrains.ReSharper.Psi.CSharp.Tree;
// using JetBrains.ReSharper.Psi.Tree;
// using ReSharperPlugin.AlphabetizeIt.Highlightings;
//
// namespace ReSharperPlugin.AlphabetizeIt.Analyzers;
//
// [ElementProblemAnalyzer(
//     ElementTypes: [typeof(IClassDeclaration)],
//     HighlightingTypes = [typeof(AbitHighlighter)])]
// public sealed class SortPropertiesInspection : IConditionalElementProblemAnalyzer
// {
//     public void Run(ITreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
//     {
//         if (element is not IClassDeclaration classDeclaration
//             || classDeclaration?.NameIdentifier is null)
//         {
//             return;
//         }
//
//
//         // if (classDeclaration.PropertyDeclarations.Count > 1)
//         // {
//         //     consumer.AddHighlighting(new AbitHighlighter(element));
//         // }
//     }
//
//     // TODO: determine if we should run by comparing the order of properties
//     public bool ShouldRun(IFile file, ElementProblemAnalyzerData data)
//     {
//         return true;
//     }
// }