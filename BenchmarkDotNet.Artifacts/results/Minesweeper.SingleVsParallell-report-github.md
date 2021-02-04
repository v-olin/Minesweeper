``` ini

BenchmarkDotNet=v0.12.1.20210204-develop, OS=Windows 10.0.18363.1316 (1909/November2019Update/19H2)
Intel Core i5-1035G7 CPU 1.20GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.101
  [Host]     : .NET 5.0.1 (5.0.120.57516), X64 RyuJIT DEBUG  [AttachedDebugger]
  DefaultJob : .NET 5.0.1 (5.0.120.57516), X64 RyuJIT


```
|   Method |     Mean |    Error |   StdDev |   Median |
|--------- |---------:|---------:|---------:|---------:|
| AsSingle | 69.40 ns | 2.670 ns | 7.308 ns | 67.32 ns |
