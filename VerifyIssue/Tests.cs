using System.Runtime.CompilerServices;

namespace VerifyIssue;

public class Tests
{
    static Tests()=>
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.DefaultValueHandling = Argon.DefaultValueHandling.Include;
            settings.ContractResolver = new Argon.CamelCasePropertyNamesContractResolver();
        });
    
    private VerifySettings GetVerifySettings(string methodName)
    {
        var settings = new VerifySettings();
        settings.UseDirectory($"./Snapshots");
        settings.UseFileName(methodName);
        settings.UseStrictJson();
        settings.DontIgnoreEmptyCollections();
        settings.IgnoreMember("traceId"); // Emitted by ASP.NET model validation
        return settings;
    }
    
    public async Task VerifyJson(string? verifyJson, Action<VerifySettings>? verifySettings = null, [CallerMemberName] string methodName = "")
    {
        var settings = GetVerifySettings(methodName);
        verifySettings?.Invoke(settings);

        await Verifier.VerifyJson(verifyJson, settings);
    }
    
    [Fact]
    public async Task MyTest()
    {
        var target = "{\"id\": \"myId_1\",\"version\":{\"obsoleteDate\":null,\"resourceVersion\":\"2018-01-01\"},\"messages\":[],}";

        await VerifyJson(target, verifySettings =>
        {
            verifySettings.IgnoreMember("id");
        });
    }
}