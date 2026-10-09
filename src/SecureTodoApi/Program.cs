using Microsoft.AspNetCore.Diagnostics;
using SecureTodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 16 * 1024);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.MaxDepth = 8);
builder.Services.AddSingleton<TodoStore>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    app.Logger.LogError(exception, "Unhandled request failure");
    await Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
        .ExecuteAsync(context);
}));

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none'");
    await next(context);
});

var todos = app.MapGroup("/todos");

todos.MapGet("/", (TodoStore store) => Results.Ok(store.List()));

todos.MapGet("/{id:guid}", (Guid id, TodoStore store) =>
    store.Get(id) is { } todo ? Results.Ok(todo) : Results.NotFound());

todos.MapPost("/", (CreateTodoRequest request, TodoStore store) =>
{
    var title = request.Title?.Trim();
    if (string.IsNullOrWhiteSpace(title) || title.Length > TodoStore.MaximumTitleLength)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.Title)] = [$"Title must contain 1 to {TodoStore.MaximumTitleLength} characters."]
        });
    }

    var todo = store.Create(title);
    return todo is null
        ? Results.Problem("The todo collection is full.", statusCode: StatusCodes.Status503ServiceUnavailable)
        : Results.Created($"/todos/{todo.Id}", todo);
});

todos.MapPut("/{id:guid}/complete", (Guid id, TodoStore store) =>
    store.Complete(id) is { } todo ? Results.Ok(todo) : Results.NotFound());

todos.MapDelete("/{id:guid}", (Guid id, TodoStore store) =>
    store.Delete(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

public partial class Program;
