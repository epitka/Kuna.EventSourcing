namespace Kuna.EventSourcing.Kurrent.Tests.TestingHelpers.XUnitHelpers;

using Xunit.Sdk;
using Xunit.v3;

public sealed class PriorityOrderer : ITestCaseOrderer
{
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
        where TTestCase : notnull, ITestCase
    {
        var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

        foreach (var testCase in testCases)
        {
            var priority = 0;

            // v3 note: if you want CLR reflection attributes, the test case should be IXunitTestCase
            if (testCase is IXunitTestCase xunitTestCase)
            {
                foreach (var attr in xunitTestCase.TestMethod
                                                  .Method
                                                  .GetCustomAttributes(typeof(TestPriorityAttribute), inherit: true)
                                                  .Cast<TestPriorityAttribute>())
                {
                    priority = attr.Priority;
                }
            }

            GetOrCreate(sortedMethods, priority).Add(testCase);
        }

        var result = new List<TTestCase>(testCases.Count);

        foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
        {
            list.Sort((x, y) =>
                          StringComparer.OrdinalIgnoreCase.Compare(
                              GetMethodName(x),
                              GetMethodName(y)));

            result.AddRange(list);
        }

        return result;
    }

    private static string GetMethodName<TTestCase>(TTestCase testCase)
        where TTestCase : notnull, ITestCase
    {
        return testCase is IXunitTestCase xunitTestCase
            ? (xunitTestCase.TestMethod?.Method?.Name ?? testCase.TestCaseDisplayName)
            : testCase.TestCaseDisplayName;
    }

    private static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var result))
        {
            return result;
        }

        result = new TValue();
        dictionary[key] = result;

        return result;
    }
}
