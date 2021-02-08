``` ini

BenchmarkDotNet=v0.12.1.20210208-develop, OS=Windows 10.0.19041.746 (2004/May2020Update/20H1)
Intel Core i5-9600K CPU 3.70GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK=5.0.101
  [Host]     : .NET 5.0.1 (5.0.120.57516), X64 RyuJIT DEBUG
  DefaultJob : .NET 5.0.1 (5.0.120.57516), X64 RyuJIT


```
|   Method |     Mean |    Error |   StdDev |
|--------- |---------:|---------:|---------:|
| AsSingle | 92.53 ns | 0.294 ns | 0.245 ns |
| AsSamuel | 45.10 ns | 0.027 ns | 0.024 ns |
