namespace Kuna.EventSourcing.Core
{
    public interface IHaveVersion
    {
       ulong? Version { get; set; }
    }
}
