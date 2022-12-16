﻿namespace Senf.EventSourcing.Core.Commands;

public interface ICommandBus
{
    Task Send<TCommand>(TCommand command) where TCommand : ICommand;
}
