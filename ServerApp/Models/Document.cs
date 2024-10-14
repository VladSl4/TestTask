using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ServerApp.Models

{
    public class Document
    {
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public int Amount { get; set; }

        [StringLength(128)]
        public string Description { get; set; } = null!;

        public ICollection<DocumentStatus> DocumentStatuses { get; set; }
    }
}
