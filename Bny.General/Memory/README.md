# Bny.General.Memory
Things that work with memory.

## In this directory
- **ConstPtr.cs:** the `ConstPtr<T>` ref struct
- **Ptr.cs:** the `Ptr<T>` ref struct

## Examples
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
