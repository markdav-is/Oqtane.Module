using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using RootNamespace.Models;

namespace RootNamespace.Repository
{
    public interface IModuleNameRepository
    {
        IEnumerable<ModuleName> GetModuleNames(int moduleId);
        ModuleName GetModuleName(int moduleNameId);
        ModuleName GetModuleName(int moduleNameId, int moduleId);
        ModuleName AddModuleName(ModuleName moduleName);
        ModuleName UpdateModuleName(ModuleName moduleName);
        void DeleteModuleName(int moduleNameId);
    }

    public class ModuleNameRepository : IModuleNameRepository, ITransientService
    {
        private readonly IDbContextFactory<ModuleNameContext> _factory;

        public ModuleNameRepository(IDbContextFactory<ModuleNameContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<ModuleName> GetModuleNames(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return db.ModuleName.Where(item => item.ModuleId == moduleId).ToList();
        }

        public ModuleName GetModuleName(int moduleNameId)
        {
            return GetModuleName(moduleNameId, -1);
        }

        public ModuleName GetModuleName(int moduleNameId, int moduleId)
        {
            using var db = _factory.CreateDbContext();
            var query = db.ModuleName.Where(item => item.ModuleNameId == moduleNameId);
            if (moduleId != -1)
            {
                query = query.Where(item => item.ModuleId == moduleId);
            }
            return query.FirstOrDefault();
        }

        public ModuleName AddModuleName(ModuleName moduleName)
        {
            using var db = _factory.CreateDbContext();
            db.ModuleName.Add(moduleName);
            db.SaveChanges();
            return moduleName;
        }

        public ModuleName UpdateModuleName(ModuleName moduleName)
        {
            using var db = _factory.CreateDbContext();
            db.ModuleName.Update(moduleName);
            db.SaveChanges();
            return moduleName;
        }

        public void DeleteModuleName(int moduleNameId)
        {
            using var db = _factory.CreateDbContext();
            var moduleName = db.ModuleName.Find(moduleNameId);
            db.ModuleName.Remove(moduleName);
            db.SaveChanges();
        }
    }
}
