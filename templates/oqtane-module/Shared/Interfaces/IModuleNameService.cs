using RootNamespace.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RootNamespace.Services
{
    public interface IModuleNameService
    {
        Task<List<ModuleName>> GetModuleNamesAsync(int moduleId);
        Task<ModuleName> GetModuleNameAsync(int moduleNameId, int moduleId);
        Task<ModuleName> AddModuleNameAsync(ModuleName moduleName);
        Task<ModuleName> UpdateModuleNameAsync(ModuleName moduleName);
        Task DeleteModuleNameAsync(int moduleNameId, int moduleId);
    }
}
