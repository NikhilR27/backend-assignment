namespace OT.Assignment.Application.Interfaces;

public interface IQueuePublisher<T>
{
    void PublishMessage(T message);
}