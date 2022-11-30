using System.Diagnostics;
namespace Senf.EventSourcing.Core.EventStore;

public interface IEventMetaDataFactory
{
    Dictionary<string, string> Get();
}

public class EventMetaDataFactory
{
    public IDictionary<string, string> Get()
    {
        var toReturn = new Dictionary<string, string>();

        if (Activity.Current?.Id != null)
        {
            toReturn.Add("CorrelationId", Activity.Current.Id);
        }

        return toReturn;
    }
}
