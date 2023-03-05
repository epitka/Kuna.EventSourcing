using Kuna.EventSourcing.Core.Commands;

namespace Kuna.EventSourcing.Core.Tests.Commands;

public class CommandDispatcherTests
{
    public record Create(Guid id) : ICommand;

    public record Update(Guid id) : ICommand;

    public record Change(Guid Id) : ICommand<string>;

    [Fact]
    public async Task Should_Invoke_Correct_Handler()
    {
        var command = new Create(Guid.NewGuid());

        var fakeCreateHandler = A.Fake<IHandleCommand<Create>>(opt => opt.Strict());
        var createCallConfig = A.CallTo(() => fakeCreateHandler.Handle(command, A<CancellationToken>._));

        createCallConfig.Returns(Task.CompletedTask);

        var fakeUpdateHandler = A.Fake<IHandleCommand<Update>>(opt => opt.Strict());

        var sc = new ServiceCollection();

        sc.AddScoped<IHandleCommand<Create>>(sp => fakeCreateHandler);
        sc.AddScoped<IHandleCommand<Update>>(sp => fakeUpdateHandler);
        sc.AddScoped<ICommandDispatcher, CommandDispatcher>();

        var sp = sc.BuildServiceProvider();

        using var scope = sp.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        await dispatcher.Send(command, CancellationToken.None);

        createCallConfig.MustHaveHappenedOnceExactly();

        A.CallTo(fakeUpdateHandler)
         .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_Invoke_Correct_Handler_That_Returns()
    {
        var command = new Change(Guid.NewGuid());

        var sc = new ServiceCollection();

        sc.AddScoped<ICommandDispatcher, CommandDispatcher>();

        sc.AddTransient<IHandleCommand<Change, string>, ChangeHandler>();

        var sp = sc.BuildServiceProvider();

        using var scope = sp.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await dispatcher.Send(command, CancellationToken.None);

        result.Should().Be(command.Id.ToString());
    }

    public class ChangeHandler : IHandleCommand<Change, string>
    {
        public Task<string> Handle(Change command, CancellationToken ct)
        {
            return Task.FromResult(command.Id.ToString());
        }
    }
}
