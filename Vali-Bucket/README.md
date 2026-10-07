# Vali-Bucket

[![NuGet](https://img.shields.io/nuget/v/Vali-Bucket.svg)](https://www.nuget.org/packages/Vali-Bucket)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](../LICENSE)

> Groups timestamps into fixed-size, aligned time buckets for reporting and analytics.

## Features

- **`Bucketize`** — group raw timestamps into fixed-size buckets, counting how many fall in each.
- **`Aggregate<T>`** — group (timestamp, value) pairs into buckets, combining values per bucket
  with `Sum`/`Average`/`Count`/`Min`/`Max`.
- **`GetBucketFor`** — O(1) lookup of the bucket a single timestamp belongs to, without
  bucketizing a whole collection — useful in a streaming hot path.
- **Alignment anchor** — buckets from separate calls with the same size and anchor are always
  comparable, because bucket boundaries are computed from the anchor, not from the data.
- Reuses `Vali-Range`'s `DateRange` for each bucket's time span.

## Installation

```bash
dotnet add package Vali-Bucket
```

## Quick Start

```csharp
using Vali_Bucket.Core;
using Vali_Bucket.Models;
using Vali_Time.Enums;

IValiBucket bucket = new ValiBucket();

var timestamps = new[]
{
    new DateTime(2025, 1, 1, 9, 2, 0),
    new DateTime(2025, 1, 1, 9, 7, 0),
    new DateTime(2025, 1, 1, 9, 16, 0),
};

var buckets = bucket.Bucketize(timestamps, new BucketSize(15, TimeUnit.Minutes));
// Two 15-minute buckets: [09:00-09:14:59.9999999] with Count=2, [09:15-09:29:59.9999999] with Count=1
```

### Aggregating numeric values

```csharp
var points = new[]
{
    (new DateTime(2025, 1, 1, 9, 2, 0), 100m),
    (new DateTime(2025, 1, 1, 9, 7, 0), 50m),
    (new DateTime(2025, 1, 1, 9, 16, 0), 30m),
};

var totals = bucket.Aggregate(points, new BucketSize(15, TimeUnit.Minutes), BucketAggregation.Sum);
// [09:00 bucket].Value == 150m, [09:15 bucket].Value == 30m
```

### Single lookup

```csharp
TimeBucket slot = bucket.GetBucketFor(DateTime.UtcNow, new BucketSize(1, TimeUnit.Hours));
```

## API Reference

| Member | Signature |
|---|---|
| `Bucketize` | `IReadOnlyList<TimeBucket> Bucketize(IEnumerable<DateTime> timestamps, BucketSize size, DateTime? anchor = null)` |
| `Aggregate<T>` | `IReadOnlyList<TimeBucket<T>> Aggregate<T>(IEnumerable<(DateTime, T)> points, BucketSize size, BucketAggregation aggregation, DateTime? anchor = null) where T : INumber<T>` |
| `GetBucketFor` | `TimeBucket GetBucketFor(DateTime timestamp, BucketSize size, DateTime? anchor = null)` |

`BucketSize(decimal Amount, TimeUnit Unit)` only supports fixed-length units —
`Milliseconds`, `Seconds`, `Minutes`, `Hours`, `Days`, `Weeks`. `Months`/`Years` have no
constant tick length and throw `ArgumentOutOfRangeException`.

Buckets with zero matching timestamps/points are omitted from `Bucketize`/`Aggregate` results
(no empty-bucket padding).

## v1 Scope

- No timezone/DST awareness — bucket boundaries are pure tick arithmetic over whatever
  `DateTime.Kind` the caller passes in. Convert to a consistent kind/zone before bucketizing
  if that matters for your data.
- No windowed/rolling aggregation (sliding windows, overlapping buckets) — buckets are
  always disjoint and grid-aligned.

## Dependency Injection

```csharp
// Program.cs
builder.Services.AddValiBucket();
```

## License

MIT © 2025 Felipe Rafael Montenegro Morriberon
