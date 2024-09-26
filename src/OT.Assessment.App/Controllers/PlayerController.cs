using Microsoft.AspNetCore.Mvc;
using OT.Assignment.Application.DTOs;
using OT.Assignment.Application.Interfaces;

namespace OT.Assessment.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly IWagerService _wagerService;

    public PlayerController(IPlayerService playerService, IWagerService wagerService)
    {
        _playerService = playerService;
        _wagerService = wagerService;
    }

    [HttpPost("casinoWager")]
    public IActionResult ReceivePlayerWagerEvent(
        [FromBody] WagerPublishedEventDto wagerPublishedEventDto)
    {
        _wagerService.PublishWagerAsync(wagerPublishedEventDto);
        return Ok();
    }
    
    [HttpGet("{playerId}/casino")]
    public async Task<IActionResult> GetPlayerWagersByPlayerId([FromRoute] int playerId, int currentPage, int pageSize)
    {
        return Ok(await _playerService.GetPaginatedWagersByPlayerId(playerId, currentPage, pageSize));
    }

    [HttpGet("topSpenders")]
    public async Task<IActionResult> GetTopSpendingPlayers([FromQuery] int count)
    {
        return Ok(await _playerService.GetTopSpendingPlayersAsync(count));
    }
}