-- Ensuring that the script is repeatable
-- Existing tables will be DELETED!!!

IF OBJECT_ID('PlayerCasinoWager', 'U') IS NOT NULL
    DROP TABLE PlayerCasinoWager;

IF OBJECT_ID('Game', 'U') IS NOT NULL
    DROP TABLE Game;

IF OBJECT_ID('Provider', 'U') IS NOT NULL
    DROP TABLE Provider;

IF OBJECT_ID('Theme', 'U') IS NOT NULL
    DROP TABLE Theme;

IF OBJECT_ID('Country', 'U') IS NOT NULL
    DROP TABLE Country;

IF OBJECT_ID('PlayerAccount', 'U') IS NOT NULL
    DROP TABLE PlayerAccount;

-- Create Provider table
IF NOT EXISTS (SELECT *
               FROM sysobjects
               WHERE name = 'Provider'
                 AND xtype = 'U')
    BEGIN
        CREATE TABLE Provider
        (
            Id   INT IDENTITY (1,1) PRIMARY KEY,
            Name NVARCHAR(100) NOT NULL
        );
    END

-- Create Theme table
IF NOT EXISTS (SELECT *
               FROM sysobjects
               WHERE name = 'Theme'
                 AND xtype = 'U')
    BEGIN
        CREATE TABLE Theme
        (
            Id   INT IDENTITY (1,1) PRIMARY KEY,
            Name NVARCHAR(100) NOT NULL
        );
    END

-- Create Country table
IF NOT EXISTS (SELECT *
               FROM sysobjects
               WHERE name = 'Country'
                 AND xtype = 'U')
    BEGIN
        CREATE TABLE Country
        (
            Id   INT IDENTITY (1,1) PRIMARY KEY,
            Name NVARCHAR(100) NOT NULL
        );
    END
    
-- Create PlayerAccount table
IF NOT EXISTS (SELECT *
               FROM sysobjects
               WHERE name = 'PlayerAccount'
                 AND xtype = 'U')
    BEGIN
        CREATE TABLE PlayerAccount
        (
            Id        INT IDENTITY (1,1) PRIMARY KEY,
            AccountId UNIQUEIDENTIFIER NOT NULL,
            Username  NVARCHAR(100)    NOT NULL UNIQUE
        );
    END

-- Create Game table
IF NOT EXISTS (SELECT *
               FROM sysobjects
               WHERE name = 'Game'
                 AND xtype = 'U')
    BEGIN
        CREATE TABLE Game
        (
            Id      INT IDENTITY (1,1) PRIMARY KEY,
            Name    NVARCHAR(100) NOT NULL,
            ThemeId INT           NOT NULL,
            CONSTRAINT FK_ThemeId_Game FOREIGN KEY (ThemeId) REFERENCES Theme (Id)
        );
    END

-- Create PlayerCasinoWager table
IF NOT EXISTS (SELECT *
               FROM sysobjects
               WHERE name = 'PlayerCasinoWager'
                 AND xtype = 'U')
    BEGIN
        CREATE TABLE PlayerCasinoWager
        (
            Id                  INT IDENTITY (1,1) PRIMARY KEY,
            WagerId             UNIQUEIDENTIFIER NOT NULL,
            PlayerId            INT              NOT NULL,
            GameId              INT              NOT NULL,
            ProviderId          INT              NOT NULL,
            ThemeId             INT              NOT NULL,
            TransactionId       UNIQUEIDENTIFIER NOT NULL,
            BrandId             UNIQUEIDENTIFIER NOT NULL,
            ExternalReferenceId UNIQUEIDENTIFIER NOT NULL,
            TransactionTypeId   UNIQUEIDENTIFIER NOT NULL,
            Amount              DECIMAL(18, 2)   NOT NULL,
            CreatedDateTime     DATETIME         NOT NULL,
            NumberOfBets        INT              NOT NULL,
            CountryId           INT              NOT NULL,
            SessionData         NVARCHAR(MAX),
            Duration            BIGINT,
            CONSTRAINT FK_PlayerId FOREIGN KEY (PlayerId) REFERENCES PlayerAccount (Id),
            CONSTRAINT FK_GameId FOREIGN KEY (GameId) REFERENCES Game (Id),
            CONSTRAINT FK_ProviderId FOREIGN KEY (ProviderId) REFERENCES Provider (Id),
            CONSTRAINT FK_ThemeId FOREIGN KEY (ThemeId) REFERENCES Theme (Id),
            CONSTRAINT FK_CountryId FOREIGN KEY (CountryId) REFERENCES Country (Id)
        );
    END

-- Create indexes for PlayerCasinoWager
IF NOT EXISTS (SELECT *
               FROM sys.indexes
               WHERE name = 'IX_PlayerId'
                 AND object_id = OBJECT_ID('PlayerCasinoWager'))
    BEGIN
        CREATE INDEX IX_PlayerId ON PlayerCasinoWager (PlayerId);
    END

IF NOT EXISTS (SELECT *
               FROM sys.indexes
               WHERE name = 'IX_CreatedDateTime'
                 AND object_id = OBJECT_ID('PlayerCasinoWager'))
    BEGIN
        CREATE INDEX IX_CreatedDateTime ON PlayerCasinoWager (CreatedDateTime);
    END

-- Stored procedure to get paginated wagers by playerId -- pagination on the repo layer for performance
-- Avoid fetching more records than we need to (if we want to do pagination in the Application layer- where it typically should be done)
IF OBJECT_ID('GetPaginatedWagersByPlayerId', 'P') IS NOT NULL
    DROP PROCEDURE GetPaginatedWagersByPlayerId;
GO

CREATE PROCEDURE GetPaginatedWagersByPlayerId @PlayerId INT,
                                              @PageNumber INT,
                                              @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRecords INT;

    SELECT @TotalRecords = COUNT(*)
    FROM PlayerCasinoWager
    WHERE PlayerId = @PlayerId;

    SELECT WagerId,
           TransactionId,
           Amount,
           CreatedDateTime,
           NumberOfBets,
           SessionData,
           Duration,
           GameId,
           ProviderId,
           ThemeId,
           CountryId
    FROM PlayerCasinoWager
    WHERE PlayerId = @PlayerId
    ORDER BY CreatedDateTime DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT @TotalRecords AS TotalRecords;
END;
GO

-- Stored procedure to get top spending players
IF OBJECT_ID('GetTopSpendingPlayers', 'P') IS NOT NULL
    DROP PROCEDURE GetTopSpendingPlayers;
GO

CREATE PROCEDURE GetTopSpendingPlayers @TopCount INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopCount) p.AccountId,
                           p.Username,
                           SUM(w.Amount) AS TotalAmountSpend
    FROM PlayerAccount p
             JOIN PlayerCasinoWager w ON p.Id = w.PlayerId
    GROUP BY p.AccountId, p.Username
    ORDER BY SUM(w.Amount) DESC;
END;
GO

-- Stored procedure to add a new player casino wager
IF OBJECT_ID('AddPlayerCasinoWager', 'P') IS NOT NULL
    DROP PROCEDURE AddPlayerCasinoWager;
GO

CREATE PROCEDURE AddPlayerCasinoWager @WagerId UNIQUEIDENTIFIER,
                                      @AccountId UNIQUEIDENTIFIER,
                                      @Username NVARCHAR(100),
                                      @GameName NVARCHAR(100),
                                      @ProviderName NVARCHAR(100),
                                      @ThemeName NVARCHAR(100),
                                      @TransactionId UNIQUEIDENTIFIER,
                                      @BrandId UNIQUEIDENTIFIER,
                                      @ExternalReferenceId UNIQUEIDENTIFIER,
                                      @TransactionTypeId UNIQUEIDENTIFIER,
                                      @Amount DECIMAL(18, 2),
                                      @CreatedDateTime DATETIME,
                                      @NumberOfBets INT,
                                      @CountryCode NVARCHAR(3),
                                      @SessionData NVARCHAR(MAX),
                                      @Duration BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PlayerId INT;
    SELECT @PlayerId = Id FROM PlayerAccount WHERE AccountId = @AccountId;

    IF @PlayerId IS NULL
        BEGIN
            INSERT INTO PlayerAccount (AccountId, Username)
            VALUES (@AccountId, @Username);

            SELECT @PlayerId = SCOPE_IDENTITY();
        END

    DECLARE @ProviderId INT;
    SELECT @ProviderId = Id FROM Provider WHERE Name = @ProviderName;

    IF @ProviderId IS NULL
        BEGIN
            INSERT INTO Provider (Name)
            VALUES (@ProviderName);

            SELECT @ProviderId = SCOPE_IDENTITY();
        END

    DECLARE @ThemeId INT;
    SELECT @ThemeId = Id FROM Theme WHERE Name = @ThemeName;

    IF @ThemeId IS NULL
        BEGIN
            INSERT INTO Theme (Name)
            VALUES (@ThemeName);

            SELECT @ThemeId = SCOPE_IDENTITY();
        END

    DECLARE @GameId INT;
    SELECT @GameId = Id FROM Game WHERE Name = @GameName;

    IF @GameId IS NULL
        BEGIN
            INSERT INTO Game (Name, ThemeId)
            VALUES (@GameName, @ThemeId);

            SELECT @GameId = SCOPE_IDENTITY();
        END

    DECLARE @CountryId INT;
    SELECT @CountryId = Id FROM Country WHERE Name = @CountryCode;

    IF @CountryId IS NULL
        BEGIN
            INSERT INTO Country (Name)
            VALUES (@CountryCode);

            SELECT @CountryId = SCOPE_IDENTITY();
        END

    INSERT INTO PlayerCasinoWager (WagerId, PlayerId, GameId, ProviderId, ThemeId, TransactionId,
                                   BrandId, ExternalReferenceId, TransactionTypeId, Amount,
                                   CreatedDateTime, NumberOfBets, CountryId, SessionData, Duration)
    VALUES (@WagerId, @PlayerId, @GameId, @ProviderId, @ThemeId, @TransactionId,
            @BrandId, @ExternalReferenceId, @TransactionTypeId, @Amount,
            @CreatedDateTime, @NumberOfBets, @CountryId, @SessionData, @Duration);
END;
GO
