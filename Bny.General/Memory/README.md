# Bny.General.Memory
Things that work with memory.

## In this directory
- **ConstPtr.cs:** the `ConstPtr<T>` ref struct
- **Ptr.cs:** the `Ptr<T>` ref struct
- **ReadOnlySpanWrapper.cs:** the `ReadOnlySpanWrapper<T>` struct
- **SpanWrapper.cs:** the `SpanWrapper<T>` struct
- **ConstPtrExtensions.cs:** extension methods for `ConstPtr<T>`
- **PtrExtensions.cs** extension methods for `Ptr<T>`
- **ConstPtrOrStream.cs** the `ConstPtrOrStream` ref struct
- **PtrOrStream.cs** the `PtrOrStream` ref struct
- **IConstPtrOrStreamImplementation.cs** the `IConstPtrOrStreamImplementation` internal interface
- **IPtrOrStreamImplementation.cs** the `IPtrOrStreamImplementation` internal interface
- **PtrPtrOrStreamImplementation.cs** the `PtrPtrOrStreamImpementation` internal class
- **StreamPtrOrStreamImplementation.cs** the `StreamPtrOrStreamImplementation` internal class

## Examples
### Ptr
Multiply all values in an array by 2 using Ptr
```C#
// The array where the values will be multiplied by 2
var array = int[] { 1, 2, 3, 4, 5, 6 };

// Iterate trough the array and multiply each value by 2
for (Ptr<int> ptr = array; ptr; ++ptr)
    ptr.Value *= 2;

// The array is modified because ptr just points to the original array
Console.WriteLine(string.Join(", ", array)); // 2, 4, 6, 8, 10, 12

/* Array can be implicitly converted to Ptr without copying the data itself (similar to Span)
 *  ... Ptr<int> ptr = array; ...
 * 
 * Ptr is true when its length is not 0, and otherwise false
 *  ...
 *  for ( ... ; ptr; ... )
 *      ...
 *
 * Ptr can be moved to the next value by incrementing it
 *  ... ++ptr ...
 *
 * The Ptr.Value property points to the first element in Ptr and can be modified
 *  ...
 *      ptr.Value *= 2;
 *  ...
 */
```

### ReadOnlySpanWrapper
Run a generic method with TSelf generic constraint and as a argument.
```C#
using Bny.General.Memory;
using System.Numerics;

int[] arr = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
Console.WriteLine(SomeClass.SumOrDefault<int>(arr));

class SomeClass
{
    // Method that should be called trough reflection
    public static T Sum<T>(ReadOnlySpan<T> span) where T : INumber<T>
    {
        T sum = T.AdditiveIdentity;
        foreach (var item in span)
            sum += item;
        return sum;
    }

    // Create wrapper for the function that takes the span wrapper as argument
    public static T SumWrapper<T>(ReadOnlySpanWrapper<T> span) where T : INumber<T>
        => Sum<T>(span);

    /// <summary>
    /// Calls the Sum function if the type T implements INumber, otherwise returns default value
    /// </summary>
    public static unsafe T? SumOrDefault<T>(ReadOnlySpan<T> span)
    {
        // Check whether the type implements the INumber interface
        if (!typeof(T).GetInterfaces().Any(p => p.FullName is not null && p.FullName.Contains("System.Numerics.INumber")))
            return default;

        // Ensure that the memory is fixed
        fixed (T* ptr = span)
        {
            // Get the method
            var method = typeof(SomeClass).GetMethod("SumWrapper")!;
            // Add the generic type
            var genericMethod = method.MakeGenericMethod(typeof(T))!;
            // create the arguments for the method call
            // you must call the wrapper because you cannot cast span to object
            // and calling the 'method.CreateDelegate<>()' would always throw exception
            // because it cannot ensure the generic constraint on the type
            // you also must call it using reflection because the INumber has the
            // TSelf generic type
            var args = new object[] { new ReadOnlySpanWrapper<T>(span) };
            // call the method
            var result = (T)genericMethod.Invoke(null, args)!;

            return result;
        }
    }
}
```

### ConstPtrOrStream
Method that can wor for both stream and span
```C#
// Create method that accepts ConstPtrOrStream as argument
int ReadIntBE(ConstPtrOrStream data)
{
    // get ConstPtr to read data
    ConstPtr<byte> p = data.Read(4);

    if (p.Length != 4)
        throw new ArgumentOutOfRangeException(nameof(data));

    // do stuff with the data
    int res = p[3];
    res |= p[2] << 8;
    res |= p[1] << 16;
    res |= p[0] << 24;
    return res;
}

/* The method is now generic and can be called with either span or stream
 * as argument with only a tiny performance overhead for both of them
 */

byte[] arr = new byte[] { 0, 0, 0, 69 };

// you can call the method with ConstPtr
Console.WriteLine(ReadIntBE(arr)); // 69

MemoryStream ms = new(arr);

// or you can call it with stream
Console.WriteLine(ReadIntBE(ms)); // 69
```
