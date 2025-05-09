using JetBrains.Annotations;
using JetBrains.TextControl.DocumentMarkup;

namespace ReSharperPlugin.AlphabetizeIt.Highlightings;

[RegisterHighlighter(
    AlphabetizeitSortId,
    GroupId = GroupId,
    ForegroundColor = "#34AFE5",
    DarkForegroundColor = "#34AFE5",
    EffectType = EffectType.DOTTED_UNDERLINE,
    Layer = HighlighterLayer.ADDITIONAL_SYNTAX)]

[PublicAPI]
public static class AbitAttributeIds
{
    public const string GroupId = "AlphabetizeIt!";

    public const string AlphabetizeitSortId = "AlphabetizeIt! Sort";
}