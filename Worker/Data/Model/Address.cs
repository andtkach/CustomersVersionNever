namespace Worker.Data.Model;

public class Address
{
    public Guid Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public Boolean Current { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}
