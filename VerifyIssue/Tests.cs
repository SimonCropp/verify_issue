using System.Runtime.CompilerServices;
using Argon;

namespace VerifyIssue;

public class Tests
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // https://github.com/VerifyTests/Verify/blob/main/docs/naming.md#derivepathinfo
        DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(projectDirectory, "Snapshots"),
                // no type name is not supported in this API
                typeName: type.Name,
                methodName: method.Name));

        // https://github.com/VerifyTests/Verify/blob/main/docs/serializer-settings.md#empty-collections-are-ignored
        VerifierSettings.DontIgnoreEmptyCollections();

        // https://github.com/VerifyTests/Verify/blob/main/docs/serializer-settings.md#usestrictjson
        VerifierSettings.UseStrictJson();

        // https://github.com/VerifyTests/Verify/blob/main/docs/serializer-settings.md#usestrictjson
        VerifierSettings.IgnoreMember("traceId");

        // https://github.com/VerifyTests/Verify/blob/main/docs/serializer-settings.md#modify-defaults
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.DefaultValueHandling = DefaultValueHandling.Include;

            // this should no be considered a supported approach. but it does work
            var resolver = (DefaultContractResolver) settings.ContractResolver!;
            resolver.NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
        });
    }

    [Fact]
    public async Task MyTest()
    {
        var target = "{\"id\": \"myId_1\",\"version\":{\"obsoleteDate\":null,\"resourceVersion\":\"2018-01-01\"},\"messages\":[],}";

        await VerifyJson(target)
            .IgnoreMember("id");
    }
}