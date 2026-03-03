using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using RootNamespace.Models;

namespace RootNamespace.Services
{
    public class ModuleNameService : ServiceBase, IModuleNameService, ITransientService
    {
        public ModuleNameService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string ApiUrl => CreateApiUrl("ModuleName");

        public async Task<List<ModuleName>> GetModuleNamesAsync(int moduleId)
        {
            var items = await GetJsonAsync<List<ModuleName>>(
                CreateAuthorizationPolicyUrl($"{ApiUrl}?moduleid={moduleId}", EntityNames.Module, moduleId),
                Enumerable.Empty<ModuleName>().ToList());
            return items.OrderBy(item => item.Name).ToList();
        }

        public async Task<ModuleName> GetModuleNameAsync(int moduleNameId, int moduleId)
        {
            return await GetJsonAsync<ModuleName>(
                CreateAuthorizationPolicyUrl($"{ApiUrl}/{moduleNameId}/{moduleId}", EntityNames.Module, moduleId));
        }

        public async Task<ModuleName> AddModuleNameAsync(ModuleName moduleName)
        {
            return await PostJsonAsync<ModuleName>(
                CreateAuthorizationPolicyUrl($"{ApiUrl}", EntityNames.Module, moduleName.ModuleId), moduleName);
        }

        public async Task<ModuleName> UpdateModuleNameAsync(ModuleName moduleName)
        {
            return await PutJsonAsync<ModuleName>(
                CreateAuthorizationPolicyUrl($"{ApiUrl}/{moduleName.ModuleNameId}", EntityNames.Module, moduleName.ModuleId), moduleName);
        }

        public async Task DeleteModuleNameAsync(int moduleNameId, int moduleId)
        {
            await DeleteAsync(
                CreateAuthorizationPolicyUrl($"{ApiUrl}/{moduleNameId}/{moduleId}", EntityNames.Module, moduleId));
        }
    }
}
