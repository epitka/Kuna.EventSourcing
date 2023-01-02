﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kuna.EventSourcing.Core.EventStore;

public class BackgroundWorker : BackgroundService
{
    private readonly ILogger<BackgroundWorker> logger;
    private readonly Func<CancellationToken, Task> perform;

    public BackgroundWorker(
        string name,
        ILogger<BackgroundWorker> logger,
        Func<CancellationToken, Task> perform)
    {
        this.Name = name;
        this.logger = logger;
        this.perform = perform;
    }

    public string Name { get; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.Run(
            async () =>
            {
                await Task.Yield();

                this.logger.LogInformation("Background worker started");

                await this.perform(stoppingToken)
                          .ConfigureAwait(false);

                this.logger.LogInformation("Background worker stopped");
            },
            stoppingToken);
}
