namespace Common.Requests.Institution
{
    public class InstitutionCreatePayload
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
