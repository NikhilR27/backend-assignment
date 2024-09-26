namespace OT.Assignment.Application.DTOs;

public class PlayerCasinoWagerDto
{
    public Guid WagerId { get; set; }
    public Guid AccountId { get; set; }
    public string Username { get; set; }
    public string GameName { get; set; }
    public string ProviderName { get; set; }
    public string ThemeName { get; set; }
    public Guid TransactionId { get; set; }
    public Guid BrandId { get; set; }
    public Guid ExternalReferenceId { get; set; }
    public Guid TransactionTypeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public int NumberOfBets { get; set; }
    public string CountryCode { get; set; }
    public string SessionData { get; set; }
    public long Duration { get; set; }
}
