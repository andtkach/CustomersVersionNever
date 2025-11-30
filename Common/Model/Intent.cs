namespace Common.Model;

public class Intent
{
    public Guid Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    
    public  States State { get; set; } = States.Unknown;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}