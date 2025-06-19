namespace AdminPanel.Models;

public class Payment
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public Client? Client { get; set; }
}
