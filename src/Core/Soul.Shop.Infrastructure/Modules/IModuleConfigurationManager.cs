namespace Soul.Shop.Infrastructure.Modules;

public interface IModuleConfigurationManager
{
    IEnumerable<ModuleInfo> GetModules();
}