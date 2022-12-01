namespace Senf.EventSourcing.Core.EventStore.Tests.XUnitHelpers;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute : Attribute
{
    public TestPriorityAttribute(int priority)
    {
        this.Priority = priority;
    }

    public int Priority { get; private set; }
}