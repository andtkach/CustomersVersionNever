namespace Common.Requests.Institution
{
    public class InstitutionUpdatePayload
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
