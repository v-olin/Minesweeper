``` ini

BenchmarkDotNet=v0.12.1.20210209-develop, OS=Windows 10.0.19041.746 (2004/May2020Update/20H1)
Intel Core i5-9600K CPU 3.70GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK=5.0.101
  [Host]     : .NET 5.0.1 (5.0.120.57516), X64 RyuJIT DEBUG
  DefaultJob : .NET 5.0.1 (5.0.120.57516), X64 RyuJIT


```
|   Method |     Mean |    Error |   StdDev |
|--------- |---------:|---------:|---------:|
| AsSingle | 95.00 ns | 0.264 ns | 0.234 ns |
|  AsQueue | 48.95 ns | 0.063 ns | 0.059 ns |
