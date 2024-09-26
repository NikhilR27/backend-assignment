using OT.Assignment.Application.DTOs;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Application.Mapping;
using OT.Assignment.Domain.Entities;

namespace OT.Assignment.Application.Services;

public class WagerService(
    IQueuePublisher<WagerPublishedEventDto> wagerPublisher,
    IPlayerCasinoRepository repository) : IWagerService
{
    public void PublishWagerAsync(WagerPublishedEventDto wagerPublishedEventDto)
    {
        wagerPublisher.PublishMessage(wagerPublishedEventDto);
    }

    public async Task SaveWagerAsync(WagerPublishedEventDto wagerPublishedEventDto)
    {
        try
        {
            var playerCasinoWager = WagerPubEventToPlayerCasinoWager.MapToPlayerCasinoWager(wagerPublishedEventDto);
            await repository.AddPlayerCasinoWagerAsync(playerCasinoWager);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}