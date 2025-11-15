namespace Common.Requests.Institution
{
    public class InstitutionPatchPayload
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = "";
        public string? Description { get; set; } = "";
    }
}
