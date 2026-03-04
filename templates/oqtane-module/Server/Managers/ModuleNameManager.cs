using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;
using RootNamespace.Models;
using RootNamespace.Repository;

namespace RootNamespace.Managers
{
    public class ModuleNameManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
    {
        private readonly IModuleNameRepository _moduleNameRepository;
        private readonly IDBContextDependencies _DBContextDependencies;

        public ModuleNameManager(IModuleNameRepository moduleNameRepository, IDBContextDependencies DBContextDependencies)
        {
            _moduleNameRepository = moduleNameRepository;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new ModuleNameContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new ModuleNameContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Oqtane.Models.Module module)
        {
            string content = "";
            var items = _moduleNameRepository.GetModuleNames(module.ModuleId).ToList();
            if (items.Any())
            {
                content = JsonSerializer.Serialize(items);
            }
            return content;
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var items = JsonSerializer.Deserialize<List<ModuleName>>(content);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        _moduleNameRepository.AddModuleName(new ModuleName
                        {
                            ModuleId = module.ModuleId,
                            Name = item.Name
                        });
                    }
                }
            }
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
            var searchContents = new List<SearchContent>();
            foreach (var item in _moduleNameRepository.GetModuleNames(pageModule.ModuleId))
            {
                if (item.ModifiedOn >= lastIndexedOn)
                {
                    searchContents.Add(new SearchContent
                    {
                        EntityName = "RootNamespaceModuleName",
                        EntityId = item.ModuleNameId.ToString(),
                        Title = item.Name,
                        Body = item.Name,
                        ContentModifiedBy = item.ModifiedBy,
                        ContentModifiedOn = item.ModifiedOn
                    });
                }
            }
            return Task.FromResult(searchContents);
        }
    }
}
