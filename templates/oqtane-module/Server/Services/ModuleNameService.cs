using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Modules;
using RootNamespace.Models;
using RootNamespace.Repository;

namespace RootNamespace.Services
{
    public class ModuleNameService : IModuleNameService, ITransientService
    {
        private readonly IModuleNameRepository _moduleNameRepository;

        public ModuleNameService(IModuleNameRepository moduleNameRepository)
        {
            _moduleNameRepository = moduleNameRepository;
        }

        public Task<List<ModuleName>> GetModuleNamesAsync(int moduleId)
        {
            return Task.FromResult((List<ModuleName>)_moduleNameRepository.GetModuleNames(moduleId));
        }

        public Task<ModuleName> GetModuleNameAsync(int moduleNameId, int moduleId)
        {
            return Task.FromResult(_moduleNameRepository.GetModuleName(moduleNameId, moduleId));
        }

        public Task<ModuleName> AddModuleNameAsync(ModuleName moduleName)
        {
            return Task.FromResult(_moduleNameRepository.AddModuleName(moduleName));
        }

        public Task<ModuleName> UpdateModuleNameAsync(ModuleName moduleName)
        {
            return Task.FromResult(_moduleNameRepository.UpdateModuleName(moduleName));
        }

        public Task DeleteModuleNameAsync(int moduleNameId, int moduleId)
        {
            _moduleNameRepository.DeleteModuleName(moduleNameId);
            return Task.CompletedTask;
        }
    }
}
