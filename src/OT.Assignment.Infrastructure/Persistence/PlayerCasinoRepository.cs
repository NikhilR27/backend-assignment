using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OT.Assignment.Application.DTOs;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Application.Responses;
using OT.Assignment.Domain.Entities;

namespace OT.Assignment.Infrastructure.Persistence;

public class PlayerCasinoRepository : IPlayerCasinoRepository
{
    private readonly IDbConnection _dbConnection;

    public PlayerCasinoRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<PlayerAccount> GetPlayerByAccountIdAsync(Guid accountId)
    {
        using var connection = CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<PlayerAccount>(
            "GetPlayerByAccountId",
            new { AccountId = accountId },
            commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task AddPlayerAsync(PlayerAccount player)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(
            "AddPlayerAccount",
            new { player.AccountId, player.Username },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<TopSpendingPlayerDto>> GetTopSpendingPlayersAsync(int topCount)
    {
        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TopSpendingPlayerDto>(
            "GetTopSpendingPlayers",
            new { TopCount = topCount },
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task<PaginatedResult<PlayerCasinoWager>> GetPaginatedWagersByPlayerIdAsync(int playerId,
        int pageNumber, int pageSize)
    {
        var paginatedResult = new PaginatedResult<PlayerCasinoWager>();

        using var connection = CreateConnection();
        await using var multi = await connection.QueryMultipleAsync(
            "GetPaginatedWagersByPlayerId",
            new { PlayerId = playerId, PageNumber = pageNumber, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);
        var wagers = (await multi.ReadAsync<PlayerCasinoWager>()).ToList();
        var totalRecords = (await multi.ReadAsync<int>()).Single();

        paginatedResult.Data = wagers;
        paginatedResult.Page = pageNumber;
        paginatedResult.PageSize = pageSize;
        paginatedResult.Total = totalRecords;
        paginatedResult.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        return paginatedResult;
    }

    public async Task AddPlayerCasinoWagerAsync(PlayerCasinoWagerDto dto)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(
            "AddPlayerCasinoWager",
            new
            {
                WagerId = dto.WagerId,
                AccountId = dto.AccountId,
                Username = dto.Username,
                GameName = dto.GameName,
                ProviderName = dto.ProviderName,
                ThemeName = dto.ThemeName,
                TransactionId = dto.TransactionId,
                BrandId = dto.BrandId,
                ExternalReferenceId = dto.ExternalReferenceId,
                TransactionTypeId = dto.TransactionTypeId,
                Amount = dto.Amount,
                CreatedDateTime = dto.CreatedDateTime,
                NumberOfBets = dto.NumberOfBets,
                CountryCode = dto.CountryCode,
                SessionData = dto.SessionData,
                Duration = dto.Duration
            },
            commandType: CommandType.StoredProcedure
        );
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_dbConnection.ConnectionString);
    }
}