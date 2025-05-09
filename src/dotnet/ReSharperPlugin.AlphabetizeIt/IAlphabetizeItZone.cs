using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugin.AlphabetizeIt;

[ZoneDefinition]
// [ZoneDefinitionConfigurableFeature("Title", "Description", IsInProductSection: false)]
public interface IAlphabetizeItZone : IZone
{
}

[ZoneMarker]
public class AbitZoneMarker : IRequire<ILanguageCSharpZone>;