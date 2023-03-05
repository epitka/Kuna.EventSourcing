namespace Kuna.EventSourcing.Core.Commands;

// marker interface for domain commands
public interface ICommand
{

}
public interface ICommand<out TReturn>
{

}
