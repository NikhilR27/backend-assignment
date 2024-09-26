namespace OT.Assignment.Domain.Entities;

public class PlayerAccount : BaseEntity
{
    public Guid AccountId { get; set; }
    public string Username { get; set; }
}