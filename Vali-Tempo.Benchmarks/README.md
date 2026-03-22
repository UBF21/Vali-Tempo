# Vali-Tempo Benchmarks

Benchmarks using [BenchmarkDotNet](https://benchmarkdotnet.org/) for the Vali-Tempo ecosystem.

## Running

```bash
cd Vali-Tempo.Benchmarks
dotnet run -c Release
```

To run a specific benchmark class:

```bash
dotnet run -c Release -- --filter *Holiday*
dotnet run -c Release -- --filter *Schedule*
dotnet run -c Release -- --filter *Conversion*
dotnet run -c Release -- --filter *Age*
```

## Benchmarks

| Class | What it measures |
|-------|-----------------|
| `HolidayBenchmarks` | `IsHoliday` cache efficiency, `GetHolidays` throughput |
| `ScheduleBenchmarks` | `Occurrences` generation speed (O(n) vs former O(n²)) |
| `ConversionBenchmarks` | `ValiTime.Convert` and `ValiDuration` arithmetic |
| `AgeBenchmarks` | Age calculation including Feb 29 edge case |
