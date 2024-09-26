using OT.Assignment.Application.DTOs;

namespace OT.Assignment.Application.Mapping;

public class WagerPubEventToPlayerCasinoWager
{
    public static PlayerCasinoWagerDto MapToPlayerCasinoWager(WagerPublishedEventDto publishedEvent)
    {
        return new PlayerCasinoWagerDto
        {
            WagerId = publishedEvent.WagerId,
            AccountId = publishedEvent.AccountId,
            Username = publishedEvent.Username,
            GameName = publishedEvent.GameName,
            ProviderName = publishedEvent.Provider,
            ThemeName = publishedEvent.Theme,
            TransactionId = publishedEvent.TransactionId,
            BrandId = publishedEvent.BrandId,
            ExternalReferenceId = publishedEvent.ExternalReferenceId,
            TransactionTypeId = publishedEvent.TransactionTypeId,
            Amount = Convert.ToDecimal(publishedEvent.Amount),
            CreatedDateTime = publishedEvent.CreatedDateTime,
            NumberOfBets = Convert.ToInt32(publishedEvent.NumberOfBets),
            CountryCode = publishedEvent.CountryCode,
            SessionData = publishedEvent.SessionData,
            Duration = Convert.ToInt64(publishedEvent.Duration)
        };
    }

    private static int GetPlayerIdByAccountId(Guid accountId) => 1;
    private static int GetGameIdByName(string gameName) => 1;
    private static int GetProviderIdByName(string providerName) => 1;
    private static int GetThemeIdByName(string themeName) => 1;
    private static int GetCountryIdByCode(string countryCode) => 1;
}