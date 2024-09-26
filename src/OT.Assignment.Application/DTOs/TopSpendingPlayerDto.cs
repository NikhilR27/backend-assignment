namespace OT.Assignment.Application.DTOs;

public class TopSpendingPlayerDto
{
    public Guid AccountId { get; set; }
    public string Username { get; set; }
    public double TotalAmountSpend { get; set; }
}