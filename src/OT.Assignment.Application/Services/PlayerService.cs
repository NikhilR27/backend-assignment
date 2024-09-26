using OT.Assignment.Application.DTOs;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Application.Responses;

namespace OT.Assignment.Application.Services;

public class PlayerService(IPlayerCasinoRepository repository) : IPlayerService
{
    public async Task<PaginatedResult<WagerByPlayerDto>> GetPaginatedWagersByPlayerId(int playerId, int page,
        int pageSize)
    {
        var paginatedWagers = await repository.GetPaginatedWagersByPlayerIdAsync(playerId, page, pageSize);

        var paginatedResult = new PaginatedResult<WagerByPlayerDto>
        {
            Page = paginatedWagers.Page,
            PageSize = paginatedWagers.PageSize,
            Total = paginatedWagers.Total,
            TotalPages = paginatedWagers.TotalPages,
            Data = paginatedWagers.Data.Select(w => new WagerByPlayerDto
            {
                WagerId = w.WagerId,
                Game = w.GameId,
                Provider = w.ProviderId,
                Amount = w.Amount,
                CreatedDate = w.CreatedDateTime
            }).ToList()
        };

        return paginatedResult;
    }

    public async Task<List<TopSpendingPlayerDto>> GetTopSpendingPlayersAsync(int count)
    {
        return await repository.GetTopSpendingPlayersAsync(count);
    }
}