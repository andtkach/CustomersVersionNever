namespace Common.Requests.Customer
{
    public class CustomerPatchPayload
    {
        public Guid Id { get; set; }
        public Guid? InstitutionId { get; set; }
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
    }
}
