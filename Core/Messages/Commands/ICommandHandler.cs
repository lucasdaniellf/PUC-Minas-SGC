namespace Core.Messages.Commands
{
    public interface ICommandHandler<in TRequest, TResponse> where TRequest : CommandRequest
    {
        public Task<TResponse> Handle(TRequest command, CancellationToken token);
    }
}
