using RootNamespace.Models;
using RootNamespace.Repository;
using RootNamespace.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RootNamespace.Managers
{
    public class ModuleNameManager : IModuleNameService
    {
        private readonly IModuleNameRepository _moduleNameRepository;

        public ModuleNameManager(IModuleNameRepository moduleNameRepository)
        {
            _moduleNameRepository = moduleNameRepository;
        }

        public async Task<List<ModuleName>> GetModuleNamesAsync(int moduleId)
        {
            return await _moduleNameRepository.GetModuleNamesAsync(moduleId);
        }

        public async Task<ModuleName> GetModuleNameAsync(int moduleNameId)
        {
            return await _moduleNameRepository.GetModuleNameAsync(moduleNameId);
        }

        public async Task<ModuleName> AddModuleNameAsync(ModuleName moduleName)
        {
            return await _moduleNameRepository.AddModuleNameAsync(moduleName);
        }

        public async Task<ModuleName> UpdateModuleNameAsync(ModuleName moduleName)
        {
            return await _moduleNameRepository.UpdateModuleNameAsync(moduleName);
        }

        public async Task DeleteModuleNameAsync(int moduleNameId)
        {
            await _moduleNameRepository.DeleteModuleNameAsync(moduleNameId);
        }
    }
}
