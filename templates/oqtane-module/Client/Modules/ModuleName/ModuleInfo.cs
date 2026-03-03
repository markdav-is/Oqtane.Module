using Oqtane.Models;
using Oqtane.Modules;

namespace RootNamespace
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "ModuleName",
            Description = "ModuleName",
            Version = "1.0.0",
            ServerManagerType = "RootNamespace.Managers.ModuleNameManager, RootNamespace.Server",
            ReleaseVersions = "1.0.0",
            Dependencies = ""
        };
    }
}
