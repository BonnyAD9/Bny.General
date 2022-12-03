namespace Bny.General.Memory;

/// <summary>
/// Extension methods for ConstPtr
/// </summary>
public static class ConstPtrExtensions
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
    /// <param name="ptr">The pointer where to search</param>
    /// <param name="value">The value to find</param>
    /// <returns>
    /// Index of the first occurence of value on an index of multiple of
    /// value.Length, if there is no such index, returns -1
    /// </returns>
    public static int IndexOfRepeat<T>(
        this ConstPtr<T> ptr  ,
             ConstPtr<T> value)
    {
        for (int i = 0; ptr.Length - i > value.Length; i += value.Length)
        {
            if (ptr[i..].StartsWith(value))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Finds the index of the first occurence of the given pointer
    /// using the default equality comparer
    /// </summary>
    /// <typeparam name="T">Type of data in the pointer</typeparam>
    /// <param name="ptr">Pointer in which to search</param>
    /// <param name="value">Value to find</param>
    /// <returns>
    /// Index of the first occurrence, -1 if there is no occurence
    /// </returns>
    public static int IndexOf<T>(this ConstPtr<T> ptr, ConstPtr<T> value)
    {
        for (int i = 0; ptr.Length >= value.Length; ++i, ++ptr)
        {
            if (ptr.StartsWith(value))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Checks whether the pointer starts with the value in the other pointer
    /// using the default equality comparer
    /// </summary>
    /// <typeparam name="T">Type of values in the pointer</typeparam>
    /// <param name="ptr">Pointer which start will be checked</param>
    /// <param name="value">Pointer that should start</param>
    /// <returns>True if ptr starts with value, otherwise false</returns>
    public static bool StartsWith<T>(this ConstPtr<T> ptr, ConstPtr<T> value)
    {
        if (ptr.Length < value.Length)
            return false;

        for (; value; ++ptr, ++value)
        {
            if (+ptr is null)
            {
                if (+value is not null)
                    return false;
                continue;
            }

            if (!ptr.Value!.Equals(+value))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Finds the index of the first occurence of the given value in the
    /// pointer using the default equality comparer
    /// </summary>
    /// <typeparam name="T">Type of value to find</typeparam>
    /// <param name="ptr">Pointer to memory where to search</param>
    /// <param name="value">Value to find</param>
    /// <returns>Index of the first occurence of value in ptr</returns>
    public static int IndexOf<T>(this ConstPtr<T> ptr, T value)
    {
        for (int i = 0; ptr; ++ptr, ++i)
        {
            if (value is null)
            {
                if (+ptr is null)
                    return i;
                continue;
            }

            if (value.Equals(+ptr))
                return i;
        }

        return -1;
    }
}
