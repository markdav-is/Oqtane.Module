using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RootNamespace.Models
{
    [Table("RootNamespaceNoDots")]
    public class ModuleName
    {
        [Key]
        public int ModuleNameId { get; set; }
        public int ModuleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
