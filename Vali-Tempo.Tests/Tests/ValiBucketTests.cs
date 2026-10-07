using FluentAssertions;
using Vali_Bucket.Core;
using Vali_Bucket.Enums;
using Vali_Bucket.Models;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiBucketTests
{
    private readonly IValiBucket _bucket = new ValiBucket();

    [Fact]
    public void Bucketize_GroupsTimestampsIntoFixedSizeBuckets()
    {
        var timestamps = new[]
        {
            new DateTime(2025, 1, 1, 9, 2, 0),
            new DateTime(2025, 1, 1, 9, 7, 0),
            new DateTime(2025, 1, 1, 9, 16, 0),
        };

        var buckets = _bucket.Bucketize(timestamps, new BucketSize(15, TimeUnit.Minutes), DateTime.MinValue);

        buckets.Should().HaveCount(2);
        buckets[0].Range.Start.Should().Be(new DateTime(2025, 1, 1, 9, 0, 0));
        buckets[0].Count.Should().Be(2);
        buckets[1].Range.Start.Should().Be(new DateTime(2025, 1, 1, 9, 15, 0));
        buckets[1].Count.Should().Be(1);
    }

    [Fact]
    public void Bucketize_BucketRange_IsInclusiveAndAdjacentToNextBucket()
    {
        var buckets = _bucket.Bucketize(
            [new DateTime(2025, 1, 1, 9, 2, 0)],
            new BucketSize(15, TimeUnit.Minutes),
            DateTime.MinValue);

        buckets.Should().ContainSingle();
        buckets[0].Range.Start.Should().Be(new DateTime(2025, 1, 1, 9, 0, 0));
        buckets[0].Range.End.Should().Be(new DateTime(2025, 1, 1, 9, 14, 59, 999).AddTicks(9999));
    }

    [Fact]
    public void Bucketize_EmptyInput_ReturnsEmptyList()
    {
        var buckets = _bucket.Bucketize([], new BucketSize(1, TimeUnit.Hours));

        buckets.Should().BeEmpty();
    }

    [Fact]
    public void Bucketize_DifferentAnchors_ProduceDifferentAlignment()
    {
        var timestamp = new DateTime(2025, 1, 1, 9, 10, 0);
        var size = new BucketSize(15, TimeUnit.Minutes);

        var defaultAligned = _bucket.GetBucketFor(timestamp, size, DateTime.MinValue);
        var shiftedAligned = _bucket.GetBucketFor(timestamp, size, new DateTime(2025, 1, 1, 9, 5, 0));

        defaultAligned.Range.Start.Should().Be(new DateTime(2025, 1, 1, 9, 0, 0));
        shiftedAligned.Range.Start.Should().Be(new DateTime(2025, 1, 1, 9, 5, 0));
    }

    [Theory]
    [InlineData(BucketAggregation.Sum, 180d)]
    [InlineData(BucketAggregation.Average, 60d)]
    [InlineData(BucketAggregation.Count, 3d)]
    [InlineData(BucketAggregation.Min, 30d)]
    [InlineData(BucketAggregation.Max, 100d)]
    public void Aggregate_CombinesValuesPerBucket(BucketAggregation aggregation, double expected)
    {
        var points = new[]
        {
            (new DateTime(2025, 1, 1, 9, 2, 0), 100d),
            (new DateTime(2025, 1, 1, 9, 7, 0), 50d),
            (new DateTime(2025, 1, 1, 9, 10, 0), 30d),
        };

        var buckets = _bucket.Aggregate(points, new BucketSize(15, TimeUnit.Minutes), aggregation, DateTime.MinValue);

        buckets.Should().ContainSingle();
        buckets[0].Count.Should().Be(3);
        buckets[0].Value.Should().Be(expected);
    }

    [Fact]
    public void Aggregate_MultipleBuckets_AggregatesEachIndependently()
    {
        var points = new[]
        {
            (new DateTime(2025, 1, 1, 9, 2, 0), 10m),
            (new DateTime(2025, 1, 1, 9, 20, 0), 25m),
        };

        var buckets = _bucket.Aggregate(points, new BucketSize(15, TimeUnit.Minutes), BucketAggregation.Sum, DateTime.MinValue);

        buckets.Should().HaveCount(2);
        buckets[0].Value.Should().Be(10m);
        buckets[1].Value.Should().Be(25m);
    }

    [Fact]
    public void GetBucketFor_ReturnsCountOfOne()
    {
        var result = _bucket.GetBucketFor(new DateTime(2025, 1, 1, 9, 2, 0), new BucketSize(15, TimeUnit.Minutes));

        result.Count.Should().Be(1);
    }

    [Fact]
    public void GetBucketFor_TimestampBeforeAnchor_StillResolvesCorrectBucket()
    {
        // Floor division must round toward negative infinity for timestamps before the anchor.
        var anchor = new DateTime(2025, 1, 1, 9, 0, 0);
        var timestamp = new DateTime(2025, 1, 1, 8, 50, 0); // 10 minutes before anchor

        var result = _bucket.GetBucketFor(timestamp, new BucketSize(15, TimeUnit.Minutes), anchor);

        result.Range.Start.Should().Be(new DateTime(2025, 1, 1, 8, 45, 0));
        result.Range.Contains(timestamp).Should().BeTrue();
    }

    [Theory]
    [InlineData(TimeUnit.Months)]
    [InlineData(TimeUnit.Years)]
    public void NonFixedLengthUnit_ThrowsArgumentOutOfRangeException(TimeUnit unit)
    {
        var act = () => _bucket.GetBucketFor(DateTime.UtcNow, new BucketSize(1, unit));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NonPositiveAmount_ThrowsArgumentOutOfRangeException()
    {
        var act = () => _bucket.GetBucketFor(DateTime.UtcNow, new BucketSize(0, TimeUnit.Minutes));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
