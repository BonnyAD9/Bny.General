using System.Runtime.CompilerServices;

namespace Bny.General.Memory;

/// <summary>
/// Extensions for the Ptr type
/// </summary>
public static class PtrExtensions
{
    /// <summary>
    /// Finds the index of the value in the fiven span that starts on multiple
    /// of the value length
    /// </summary>
    /// <example>
    /// if the ptr would be 'abbcdd' and the value 'dd', the function
    /// would return 4, but if the value would be 'bb' the function would
    /// return -1 because 'bb' doesn't occur on index of multiple of the
    /// value.Length (2)
    /// </example>
    /// <typeparam name="T">Type of data in the pointers</typeparam>
    /// <param name="self">The pointer where to search</param>
    /// <param name="value">The value to find</param>
    /// <returns>
    /// Index of the first occurence of value on an index of multiple of
    /// value.Length, if there is no such index, returns -1
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfRepeat<T>(this Ptr<T> self, ConstPtr<T> value)
        => ((ConstPtr<T>)self).IndexOfRepeat(value);

    /// <summary>
    /// Finds the index of the first occurence of the given pointer
    /// using the default equality comparer
    /// </summary>
    /// <typeparam name="T">Type of data in the pointer</typeparam>
    /// <param name="self">Pointer in which to search</param>
    /// <param name="value">Value to find</param>
    /// <returns>
    /// Index of the first occurrence, -1 if there is no occurence
    /// </returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Ptr<T> self, ConstPtr<T> value)
        => ((ConstPtr<T>)self).IndexOf(value);

    /// <summary>
    /// Checks whether the pointer starts with the value in the other pointer
    /// using the default equality comparer
    /// </summary>
    /// <typeparam name="T">Type of values in the pointer</typeparam>
    /// <param name="self">Pointer which start will be checked</param>
    /// <param name="value">Pointer that should start</param>
    /// <returns>True if ptr starts with value, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this Ptr<T> self, ConstPtr<T> value)
        => ((ConstPtr<T>)self).StartsWith(value);


    /// <summary>
    /// Finds the index of the first occurence of the given value in the
    /// pointer using the default equality comparer
    /// </summary>
    /// <typeparam name="T">Type of value to find</typeparam>
    /// <param name="ptr">Pointer to memory where to search</param>
    /// <param name="value">Value to find</param>
    /// <returns>Index of the first occurence of value in ptr</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Ptr<T> ptr, T value)
        => ((ConstPtr<T>)ptr).IndexOf(value);

    /// <summary>
    /// Checks whether the two pointers have the same contents
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns>True if the pointers have the same content</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(this Ptr<T> p1, ConstPtr<T> p2)
        => ((ConstPtr<T>)p1).Equals(p2);

    /// <summary>
    /// Tries to data from self to ptr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="dest">Where to copy to</param>
    /// <returns>True on success, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(this Ptr<T> self, Ptr<T> dest)
        => ((ReadOnlySpan<T>)self).TryCopyTo(dest);

    /// <summary>
    /// Copies data from self to ptr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="dest">Where to copy to</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this Ptr<T> self, Ptr<T> dest)
        => ((ReadOnlySpan<T>)self).CopyTo(dest);
}
