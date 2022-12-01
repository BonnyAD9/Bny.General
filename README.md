# Bny.General
C# library with generally useful stuff.

## In this repository
- **Bny.General:** the library
- **Bny.General.Tester:** unit tests

## Library features
### Bny.General.Memory
Things that work with memory.

Features:
- `ConstPtr<T>` pointer to constant memory, simillar to `ReadOnlySpan` but has pointer arithmetic and other coll stuff
- `Ptr<T>` pointer to memory, simillar to `Span<T>` but has pointer arighmetic and other cool stuff
- `ReadOnlySpanWrapper<T>` wrapper for `ReadOnlySpan<T>` that can be casted to object
- `SpanWrapper<T>` wrapper for `Span<T>` that can be casted to object

## How to get it
This library is available as a [NuGet package](https://www.nuget.org/packages/Bny.General/).

## Links
- **Author:** [BonnyAD9](https://github.com/BonnyAD9)
- **GitHub repository:** [Bny.General](https://github.com/BonnyAD9/Bny.General)
- **NuGet package:** [Bny.General](https://www.nuget.org/packages/Bny.General/)
- **My website:** [bonnyad9.github.io](https://bonnyad9.github.io/)
- **Documentation:** [Doxygen](https://bonnyad9.github.io/Bny.General/)
