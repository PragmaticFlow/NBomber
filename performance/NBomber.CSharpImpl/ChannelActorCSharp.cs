using System.Threading.Channels;

namespace NBomber.CSharpImpl;

public class ChannelActorCSharp<TState,TMessage>
{
    TState _currentState;
    private bool stop = false;
    private readonly Func<TState, TMessage, TState> _handler;
    private readonly Channel<TMessage> _channel = Channel.CreateUnbounded<TMessage>();

    public ChannelActorCSharp(TState initialState, Func<TState, TMessage, TState> handler)
    {
        _currentState = initialState;
        _handler = handler;

        Loop();
    }

    async Task Loop()
    {
        while (!stop)
        {
            var msg = await _channel.Reader.ReadAsync();
            _currentState = _handler(_currentState, msg);
        }
    }

    public void Publish(TMessage message) => _channel.Writer.TryWrite(message);
}
