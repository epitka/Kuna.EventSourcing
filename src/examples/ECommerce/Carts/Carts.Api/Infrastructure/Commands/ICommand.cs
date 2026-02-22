namespace Carts.Infrastructure.Commands;

// marker interface for domain commands
public interface ICommand
{

}
public interface ICommand<out TReturn>
{

}
