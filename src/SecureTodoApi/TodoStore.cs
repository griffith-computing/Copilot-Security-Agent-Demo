using System.Collections.Concurrent;

namespace SecureTodoApi;

public sealed class TodoStore
{
    public const int MaximumTitleLength = 200;
    private const int MaximumItemCount = 1_000;
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();
    private readonly object _writeLock = new();

    public IReadOnlyCollection<TodoItem> List() =>
        _items.Values.OrderBy(item => item.CreatedAt).Take(100).ToArray();

    public TodoItem? Get(Guid id) =>
        _items.GetValueOrDefault(id);

    public TodoItem? Create(string title)
    {
        lock (_writeLock)
        {
            if (_items.Count >= MaximumItemCount)
            {
                return null;
            }

            var item = new TodoItem(Guid.NewGuid(), title, false, DateTimeOffset.UtcNow);
            _items[item.Id] = item;
            return item;
        }
    }

    public TodoItem? Complete(Guid id)
    {
        lock (_writeLock)
        {
            if (!_items.TryGetValue(id, out var current))
            {
                return null;
            }

            var updated = current with { IsComplete = true };
            _items[id] = updated;
            return updated;
        }
    }

    public bool Delete(Guid id) =>
        _items.TryRemove(id, out _);
}

public sealed record TodoItem(
    Guid Id,
    string Title,
    bool IsComplete,
    DateTimeOffset CreatedAt);

public sealed record CreateTodoRequest(string? Title);
