namespace ServerApp.Models
{
    public class DocumentStatus
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }

        public int StatusId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Document Document { get; set; }

        public Status Status { get; set; }
    }
}
