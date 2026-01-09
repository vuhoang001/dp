
namespace DesignPattern.BehaviourPattern.Cor.Solution;

public enum HandlerResult { Handled,
    Continue,
    Skip
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{Severity}] {Field}: {Message}";
    }
}

public class Request<TData>(TData data, Dictionary<string, object>? context = null)
{
    public TData Data { get; set; } = data;
    public Dictionary<string, object> Context { get; set; } = context ?? new Dictionary<string, object>();
    private object? Result { get; set; }
    // ReSharper disable once CollectionNeverQueried.Local
    private Dictionary<string, object> Metadata { get; set; } = new();

    public void SetResult(object res) => this.Result = res;

    public T GetResult<T>()
    {
        if (Result is null) throw new InvalidOperationException("Result not set.");
        return (T)Result;
    }

    public void AddMetadata(string key, object value) => Metadata[key] = value;
}

public abstract class Handler<TData>
{
    private Handler<TData>? _nextHandler;
    protected string Name { get; set; }

    protected Handler() => Name = GetType().Name;

    public void SetNext(Handler<TData>? nextHandler) => _nextHandler = nextHandler;

    public virtual HandlerResult Handle(Request<TData> request)
    {
        if (!CanHandle(request))
            return _nextHandler?.Handle(request) ?? HandlerResult.Handled;

        var result = Process(request);

        if (result == HandlerResult.Handled || _nextHandler is null)
            return result;

        return result switch
        {
            HandlerResult.Skip or HandlerResult.Continue => _nextHandler.Handle(request),
            _ => result
        };
    }

    public abstract HandlerResult Process(Request<TData> request);

    protected virtual bool CanHandle(Request<TData> request) => true;
}

public class Chain<TData>
{
    private readonly List<Handler<TData>> _handlers = [];
    private          Handler<TData>?      _head;

    public Chain<TData> AddHandler(Handler<TData> handler)
    {
        _handlers.Add(handler);
        RebuildChain();
        return this;
    }

    public Chain<TData> AddHandlers(params Handler<TData>[] handlers)
    {
        _handlers.AddRange(handlers);
        RebuildChain();
        return this;
    }

    public Chain<TData> RemoveHandler(Handler<TData> handler)
    {
        _handlers.Remove(handler);
        RebuildChain();
        return this;
    }

    public Chain<TData> RemoveHandler<THandler>() where THandler : Handler<TData>
    {
        _handlers.RemoveAll(h => h is THandler);
        RebuildChain();
        return this;
    }

    private void RebuildChain()
    {
        if (_handlers.Count == 0)
        {
            _head = null;
            return;
        }

        _head = _handlers[0];
        for (var i = 0; i < _handlers.Count - 1; i++)
        {
            _handlers[i].SetNext(_handlers[i + 1]);
        }
    }

    public HandlerResult Execute(Request<TData> request)
    {
        return _head?.Handle(request) ?? throw new InvalidOperationException("Chain is empty. Add handlers first.");
    }

    public HandlerResult Execute(TData data, Dictionary<string, object>? context = null)
    {
        var request = new Request<TData>(data, context);
        return Execute(request);
    }

    public Handler<TData>? Build()
    {
        RebuildChain();
        return _head;
    }

    public IReadOnlyList<Handler<TData>> GetHandlers() => _handlers.AsReadOnly();

    public void Clear()
    {
        _handlers.Clear();
        _head = null;
    }
}