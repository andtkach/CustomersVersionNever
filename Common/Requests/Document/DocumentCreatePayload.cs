namespace Common.Requests.Document
{
    public class DocumentCreatePayload
    {
        public Guid Id { get; set; }
        public Guid InstitutionId { get; set; }
        public Guid CustomerId { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public bool Active { get; set; }
    }
}
