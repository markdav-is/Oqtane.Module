using Microsoft.EntityFrameworkCore;
using Oqtane.Infrastructure;
using RootNamespace.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RootNamespace.Repository
{
    public interface IModuleNameRepository
    {
        Task<List<ModuleName>> GetModuleNamesAsync(int moduleId);
        Task<ModuleName> GetModuleNameAsync(int moduleNameId);
        Task<ModuleName> AddModuleNameAsync(ModuleName moduleName);
        Task<ModuleName> UpdateModuleNameAsync(ModuleName moduleName);
        Task DeleteModuleNameAsync(int moduleNameId);
    }

    public class ModuleNameRepository : IModuleNameRepository
    {
        private readonly IDbContextFactory<TenantDBContext> _dbContextFactory;

        public ModuleNameRepository(IDbContextFactory<TenantDBContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<ModuleName>> GetModuleNamesAsync(int moduleId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return await db.Set<ModuleName>()
                .Where(m => m.ModuleId == moduleId && !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<ModuleName> GetModuleNameAsync(int moduleNameId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return await db.Set<ModuleName>()
                .FirstOrDefaultAsync(m => m.ModuleNameId == moduleNameId && !m.IsDeleted);
        }

        public async Task<ModuleName> AddModuleNameAsync(ModuleName moduleName)
        {
            moduleName.IsDeleted = false;
            using var db = _dbContextFactory.CreateDbContext();
            db.Set<ModuleName>().Add(moduleName);
            await db.SaveChangesAsync();
            return moduleName;
        }

        public async Task<ModuleName> UpdateModuleNameAsync(ModuleName moduleName)
        {
            using var db = _dbContextFactory.CreateDbContext();

            // Load the existing entity to avoid blindly overwriting all fields on a detached instance
            var existing = await db.Set<ModuleName>()
                .FirstOrDefaultAsync(m => m.ModuleNameId == moduleName.ModuleNameId && !m.IsDeleted);

            if (existing == null)
            {
                // No matching, non-deleted record found; let caller handle this case
                return null;
            }

            // Preserve soft-delete status to avoid unintentionally changing it
            var originalIsDeleted = existing.IsDeleted;

            // Update the tracked entity with values from the incoming entity
            db.Entry(existing).CurrentValues.SetValues(moduleName);

            // Restore the original soft-delete flag
            existing.IsDeleted = originalIsDeleted;

            await db.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteModuleNameAsync(int moduleNameId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var moduleName = await db.Set<ModuleName>().FindAsync(moduleNameId);
            if (moduleName != null)
            {
                moduleName.IsDeleted = true;
                await db.SaveChangesAsync();
            }
        }
    }
}
