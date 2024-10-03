using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ServerApp.Models

{
    public class Document
    {
        public int Id { get; set; }

        public int Amount { get; set; }

        [StringLength(128)]
        public string Description { get; set; }

        public ICollection<DocumentStatus> DocumentStatuses { get; set; }
    }
}
