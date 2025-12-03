namespace Common.Model;

public class Consumer
{
    public string MessageId { get; set; } = string.Empty;
    public string ConsumerName { get; set; } = string.Empty;
    public DateTime ConsumedAtUtc { get; set; }
}