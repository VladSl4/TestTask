using ServerApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models
{
    public class Status
    {
        public int Id { get; set; }

        public StatusEnum Name { get; set; }

        [StringLength(128)]
        public string? Description { get; internal set; }
    }
}
