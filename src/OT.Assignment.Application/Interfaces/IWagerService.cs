using OT.Assignment.Application.DTOs;

namespace OT.Assignment.Application.Interfaces;

public interface IWagerService
{
    void PublishWagerAsync(WagerPublishedEventDto wagerPublishedEventDto);
    Task SaveWagerAsync(WagerPublishedEventDto wagerPublishedEventDto);
}