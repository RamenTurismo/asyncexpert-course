``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.959 (1909/November2018Update/19H2)
Intel Core i5-6500 CPU 3.20GHz (Skylake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.301
  [Host]     : .NET Core 3.1.5 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.27001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.5 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.27001), X64 RyuJIT


```
|               Method |       Mean |     Error |    StdDev |
|--------------------- |-----------:|----------:|----------:|
| ExecuteSynchronously |   5.213 μs | 0.0264 μs | 0.0247 μs |
|      ExecuteOnThread | 186.174 μs | 3.7114 μs | 9.6465 μs |
|  ExecuteOnThreadPool |  13.541 μs | 0.0324 μs | 0.0287 μs |
