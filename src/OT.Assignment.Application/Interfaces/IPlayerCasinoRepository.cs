using OT.Assignment.Application.DTOs;
using OT.Assignment.Application.Responses;
using OT.Assignment.Domain.Entities;

namespace OT.Assignment.Application.Interfaces;

public interface IPlayerCasinoRepository
{
    Task<PlayerAccount> GetPlayerByAccountIdAsync(Guid accountId);
    Task AddPlayerAsync(PlayerAccount player);
    Task<List<TopSpendingPlayerDto>> GetTopSpendingPlayersAsync(int topCount);

    Task<PaginatedResult<PlayerCasinoWager>> GetPaginatedWagersByPlayerIdAsync(int playerId, int pageNumber,
        int pageSize);

    Task AddPlayerCasinoWagerAsync(PlayerCasinoWagerDto dto);
}
