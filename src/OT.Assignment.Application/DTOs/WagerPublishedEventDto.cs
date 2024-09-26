namespace OT.Assignment.Application.DTOs;

public record WagerPublishedEventDto(
    Guid WagerId,
    string Theme,
    string Provider,
    string GameName,
    Guid TransactionId,
    Guid BrandId,
    Guid AccountId,
    string Username,
    Guid ExternalReferenceId,
    Guid TransactionTypeId,
    double Amount,
    DateTime CreatedDateTime,
    int NumberOfBets,
    string CountryCode,
    string SessionData,
    long Duration
);
