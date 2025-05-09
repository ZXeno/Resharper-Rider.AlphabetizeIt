using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.AlphabetizeIt.Tests
{
    [ZoneDefinition]
    public class AlphabetizeItTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IAlphabetizeItZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<AlphabetizeItTestEnvironmentZone> { }

    [SetUpFixture]
    public class AlphabetizeItTestsAssembly : ExtensionTestEnvironmentAssembly<AlphabetizeItTestEnvironmentZone> { }
}
