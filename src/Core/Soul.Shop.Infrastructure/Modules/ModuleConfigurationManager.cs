using Newtonsoft.Json;

namespace Soul.Shop.Infrastructure.Modules;

public class ModuleConfigurationManager : IModuleConfigurationManager
{
    private const string ModulesFileName = "appsettings.Modules.json";

    public IEnumerable<ModuleInfo> GetModules()
    {
        var modules = new List<ModuleInfo>();
        var modulesPath = Path.Combine(GlobalConfiguration.ContentRootPath, ModulesFileName);
        using var reader = new StreamReader(modulesPath);
        var content = reader.ReadToEnd();
        modules = JsonConvert.DeserializeObject<List<ModuleInfo>>(content);
        return modules;
    }
}