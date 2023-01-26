# Bny.General.ErrorHandling
Types for propagating errors and dealing with them.

## In this directory
- **Result.cs** the `Result` class
- **ResultEx.cs** the `ResultEx<EX>` class
- **Result-T.cs** the `Result<T>` class
- **ResultEx-T.cs** the `Result<T, Ex>` class

## Examples
### Result
Implement conversion value functions with the result as return value.
```C#
// have result with the return type as parameter
Result<int> TryReadIntBE(ReadOnlySpan<byte> data)
{
    if (data.Length < 4)
        // instead of throwing exceptions return result with failure
        // and gain performance (throwing exceptions is expensive)
        return new Result<int, ArgumentOutOfRangeException>(nameof(data));

    int res = data[3];
    res |= data[2] << 8;
    res |= data[1] << 16;
    res |= data[0] << 24;
    return res;
}

// now if you implement function that should throw instead of returning
// failure, just throw or get the value
// in this case the throw exception will be the ArgumentOutOfRangeException
// because that is the only exception type that the original method
// returns in the result
int ReadIntBE(ReadOnlySpan<byte> data) => TryReadIntBE(data).GetOrThrow();


byte[] arr = new byte[] { 0, 0, 0, 69 };


Result<int> res = TryReadIntBE(arr).Value;

// if you want the value you can ask if the result is successful
// and get the value
if (res)
    // res.Value will never throw,
    // but it may be the default value of that type
    Console.WriteLine(res.Value); // 69

// casting the result to the value type is the same as calling
// res.GetOrThrow()
Console.WriteLine((int)res);
```