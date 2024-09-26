using OT.Assignment.Application.DTOs;
using OT.Assignment.Application.Responses;

namespace OT.Assignment.Application.Interfaces;

public interface IPlayerService
{
    Task<PaginatedResult<WagerByPlayerDto>> GetPaginatedWagersByPlayerId(int playerId, int page, int pageSize);
    Task<List<TopSpendingPlayerDto>> GetTopSpendingPlayersAsync(int count);
}