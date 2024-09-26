namespace OT.Assignment.Application.Interfaces;

public interface IQueueConsumer
{
    Task ConsumeMessage(CancellationToken cancellationToken);
}