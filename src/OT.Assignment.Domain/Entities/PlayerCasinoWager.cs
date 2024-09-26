namespace OT.Assignment.Domain.Entities;

public class PlayerCasinoWager : BaseEntity
{
    public Guid WagerId;
    public int ThemeId;
    public int ProviderId;
    public int GameId;
    public Guid TransactionId;
    public Guid BrandId;
    public Guid AccountId;
    public Guid ExternalReferenceId;
    public Guid TransactionTypeId;
    public double Amount;
    public DateTime CreatedDateTime;
    public int NumberOfBets;
    public int CountryId;
    public string SessionData;
    public long Duration;
}