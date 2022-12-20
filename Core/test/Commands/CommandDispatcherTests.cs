using Senf.EventSourcing.Core.Commands;

namespace Senf.EventSourcing.Core.Tests.Commands;

public class CommandDispatcherTests
{
    public record Create(Guid id) : ICommand;

    public record Update(Guid id) : ICommand;

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
}
