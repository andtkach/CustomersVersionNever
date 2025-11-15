namespace Common.Requests.Address
{
    public class AddressPatchPayload
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? InstitutionId { get; set; }
        public string? Country{ get; set; } = "";
        public string? City { get; set; } = "";
        public string? Street { get; set; } = "";
        public bool? Current { get; set; }
    }
}
