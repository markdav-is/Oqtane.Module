using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace RootNamespace.Models
{
    [Table("RootNamespaceModuleName")]
    public class ModuleName : ModelBase
    {
        [Key]
        public int ModuleNameId { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
    }
}
