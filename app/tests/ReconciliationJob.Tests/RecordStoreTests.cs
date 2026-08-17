using ReconciliationJob;
using Xunit;

namespace ReconciliationJob.Tests;

public class RecordStoreTests
{
    private static DateTime Utc(int year, int month, int day, int hour = 0) =>
        new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void GetRecordsForWindow_ReturnsRecordsSpreadThroughoutWindow()
    {
        var records = new List<Record>
        {
            new Record("r0", Utc(2026, 1, 1, 4), 10m),
            new Record("r1", Utc(2026, 1, 1, 12), 10m),
            new Record("r2", Utc(2026, 1, 1, 20), 10m),
        };
        var store = new RecordStore(records);

        var result = store.GetRecordsForWindow(Utc(2026, 1, 1), Utc(2026, 1, 2));

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetRecordsForWindow_IncludesRecordsAtUpperBound()
    {
        var records = new List<Record>
        {
            new Record("r0", Utc(2026, 1, 1, 4), 10m),
            new Record("r1", Utc(2026, 1, 2), 10m),
        };
        var store = new RecordStore(records);

        var result = store.GetRecordsForWindow(Utc(2026, 1, 1), Utc(2026, 1, 2));

        Assert.Equal(2, result.Count);
        Assert.Contains(records[1], result);
    }

    [Fact]
    public void GetRecordsForWindow_IncludesRecordsAtLowerBound()
    {
        var records = new List<Record>
        {
            new Record("r0", Utc(2026, 1, 1), 10m),
            new Record("r1", Utc(2026, 1, 1, 12), 10m),
        };
        var store = new RecordStore(records);

        var result = store.GetRecordsForWindow(Utc(2026, 1, 1), Utc(2026, 1, 2));

        Assert.Equal(2, result.Count);
        Assert.Contains(records[0], result);
    }

    [Fact]
    public void GetRecordsForWindow_IncludesEntireBatchAtBoundary()
    {
        var records = new List<Record>
        {
            new Record("b1r1", Utc(2026, 1, 1), 10m),
            new Record("b1r2", Utc(2026, 1, 1), 20m),
            new Record("b1r3", Utc(2026, 1, 1), 30m),
            new Record("b2r1", Utc(2026, 1, 2), 40m),
            new Record("b2r2", Utc(2026, 1, 2), 50m),
            new Record("b2r3", Utc(2026, 1, 2), 60m),
        };
        var store = new RecordStore(records);

        var result = store.GetRecordsForWindow(Utc(2026, 1, 1), Utc(2026, 1, 2));

        Assert.Equal(6, result.Count);
        foreach (var record in records)
        {
            Assert.Contains(record, result);
        }
    }
}
