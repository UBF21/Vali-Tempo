using BenchmarkDotNet.Running;
using Vali_Tempo.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
