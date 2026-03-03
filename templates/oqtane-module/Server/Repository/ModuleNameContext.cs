using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Repository.Databases.Interfaces;
using RootNamespace.Models;

namespace RootNamespace.Repository
{
    public class ModuleNameContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<ModuleName> ModuleName { get; set; }

        public ModuleNameContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ModuleName>().ToTable(ActiveDatabase.RewriteName("RootNamespaceModuleName"));
        }
    }
}
