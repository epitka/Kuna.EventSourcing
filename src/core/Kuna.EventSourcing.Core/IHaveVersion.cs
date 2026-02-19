namespace Kuna.EventSourcing.Core
{
    public interface IHaveVersion
    {
       int Version { get; set; }
    }
}
