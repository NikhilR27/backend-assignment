namespace OT.Assignment.Application.DTOs;

public class WagerByPlayerDto
{
    public Guid WagerId { get; set; }
    public int Game { get; set; }
    public int Provider { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedDate { get; set; }
}