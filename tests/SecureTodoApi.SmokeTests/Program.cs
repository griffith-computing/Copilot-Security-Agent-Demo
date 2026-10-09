using SecureTodoApi;

var store = new TodoStore();
var created = store.Create("Review the API");

Assert(created is not null, "A valid todo should be created.");
Assert(store.Get(created!.Id) == created, "The created todo should be retrievable.");
Assert(store.List().Count == 1, "The todo should appear in the list.");

var completed = store.Complete(created.Id);
Assert(completed?.IsComplete == true, "The todo should be marked complete.");
Assert(store.Delete(created.Id), "The todo should be deleted.");
Assert(store.Get(created.Id) is null, "The deleted todo should not be retrievable.");

Console.WriteLine("All smoke tests passed.");

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
