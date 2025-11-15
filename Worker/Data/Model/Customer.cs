namespace Worker.Data.Model;

public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid InstitutionId { get; set; }
    public Institution Institution { get; set; } = null!;


}
