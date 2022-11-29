using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bny.General.Memory;

/// <summary>
/// Read only version of Ptr. Represents a countinuous region of read only memory. Same as ReadOnlySpan, but with pointer arithmetic operator overloads.
/// </summary>
/// <typeparam name="T">Type of data in the memory</typeparam>
public readonly ref struct ConstPtr<T>
{
    internal readonly ref T _ptr;
    internal readonly int _length;

    /// <summary>
    /// Lenfth of the memory
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <summary>
    /// The value at the first position
    /// </summary>
    public T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => +this;
    }

    /// <summary>
    /// Gets the value at the given index
    /// </summary>
    /// <param name="index">index of the value to get</param>
    /// <returns>Item at the given index</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of range</exception>
    public T this[int index]
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (uint)index < (uint)_length ? At(index) : throw new IndexOutOfRangeException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ConstPtr(ref T ptr, int length)
    {
        if (length < 0)
            throw new IndexOutOfRangeException();
        _length = length;
        _ptr = ref ptr;
    }

    /// <summary>
    /// Creates ConstPtr from ReadOnlySpan
    /// </summary>
    /// <param name="span">ReadOnlySpan to create the ConstPtr from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(ReadOnlySpan<T> span) : this(ref MemoryMarshal.GetReference(span), span.Length) { }

    /// <summary>
    /// Creates ConstPtr from Span
    /// </summary>
    /// <param name="span">Span to create the ConstPtr from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(Span<T> span) : this(ref MemoryMarshal.GetReference(span), span.Length) { }

    /// <summary>
    /// Creates ConstPtr from an array
    /// </summary>
    /// <param name="arr">The array to create the ConstPtr from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(T[] arr) : this(ref MemoryMarshal.GetArrayDataReference(arr), arr.Length) { }

    /// <summary>
    /// Creates ConstPtr from a Ptr
    /// </summary>
    /// <param name="ptr">Ptr to create the ConstPtr from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(Ptr<T> ptr) : this(ref ptr._ptr, ptr._length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref T At(int index) => ref Unsafe.Add(ref _ptr, index);

    /// <summary>
    /// Returns slice of the data in this ConstPtr
    /// </summary>
    /// <param name="start">Start index of the slice</param>
    /// <param name="length">Size of the slice</param>
    /// <returns>New ConstPtr over the specified range of the original ConstPtr</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the requested slice is out of the range</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr<T> Slice(int start, int length) => (uint)start >= (uint)_length || (uint)(start + length) > (uint)_length
        ? throw new IndexOutOfRangeException()
        : new(ref At(start), length);

    /// <summary>
    /// Returns reference to the first item in the memory
    /// </summary>
    /// <returns>Reference to the firs item in the memory</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T GetPinnableReference() => ref _ptr;

    /// <summary>
    /// Concerts Ptr to ConstPtr (same as the ConstPtr(Ptr) constructor)
    /// </summary>
    /// <param name="ptr">The Ptr to convert to ConstPtr</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(Ptr<T> ptr) => new(ptr);

    /// <summary>
    /// Converts ConstPtr to ReadOnlySpan
    /// </summary>
    /// <param name="ptr">The ConstPtr to convert</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(ConstPtr<T> ptr) => MemoryMarshal.CreateSpan(ref ptr._ptr, ptr._length);

    /// <summary>
    /// Converts ReadOnlySpan to ConstPtr (same as the ConstPtr(ReadOnlySpan) constructor)
    /// </summary>
    /// <param name="span">The ReadOnlySpan to convert</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(ReadOnlySpan<T> span) => new(span);

    /// <summary>
    /// Converts Span to ConstPtr (same as the ConstPtr(Span) constructor)
    /// </summary>
    /// <param name="span">The Span to convert</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(Span<T> span) => new(span);

    /// <summary>
    /// Converts Array to ConstPtr (Same as the ConstPtr(Array) constructor)
    /// </summary>
    /// <param name="arr">The Array to convert</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(T[] arr) => new(arr);

    /// <summary>
    /// Increments the ConstPtr ('++ptr' has the same effect as 'ptr = ptr[1..]')
    /// </summary>
    /// <param name="ptr">ConstPtr to increment</param>
    /// <returns>The incremented ConstPtr</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the ptr length is 0</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstPtr<T> operator ++(ConstPtr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : new(ref ptr.At(1), ptr._length - 1);

    /// <summary>
    /// Determines whether the given ptr is not empty (has the same effect as ptr.Length != 0)
    /// </summary>
    /// <param name="ptr">ConstPtr to check if is empty</param>
    /// <returns>True if ptr is not empty, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(ConstPtr<T> ptr) => ptr.Length != 0;

    /// <summary>
    /// Determines whether the given ptr is empty (has the same effect as ptr.Length == 0)
    /// </summary>
    /// <param name="ptr">ConstPtr to check if it is empty</param>
    /// <returns>True if ptr is empty, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(ConstPtr<T> ptr) => ptr.Length == 0;

    /// <summary>
    /// Determines whether the given ptr is empty (has the same effect as ptr.Length == 0)
    /// </summary>
    /// <param name="ptr">ConstPtr to check if it is empty</param>
    /// <returns>True if ptr is empty, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(ConstPtr<T> ptr) => ptr.Length == 0;

    /// <summary>
    /// Adds the given number to the ConstPtr ('ptr + 5' has the same effect as 'ptr[5..]')
    /// </summary>
    /// <param name="ptr">ConstPtr to add the value to</param>
    /// <param name="value">Value to be added to the ConstPtr</param>
    /// <returns>ConstPtr with the given offset</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the added value is larger than the ptr.Length</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstPtr<T> operator +(ConstPtr<T> ptr, int value) => (uint)value < (uint)ptr._length ? new(ref ptr.At(value), ptr._length - value) : throw new IndexOutOfRangeException();

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    /// <summary>
    /// Determines the number of elements bethween the given pointers that point to the same continuous block of memory. If the pointers not't point to the same memory, the behaviour is undefined.
    /// </summary>
    /// <param name="p1">The second part of memory</param>
    /// <param name="p2">The first part of memory</param>
    /// <returns>Number of elements betwen the memory pointers</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(ConstPtr<T> p1, ConstPtr<T> p2) => (int)Unsafe.ByteOffset(ref p2._ptr, ref p1._ptr) / sizeof(T);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

    /// <summary>
    /// Gets the value that the pointer points to (Same as ptr.Value)
    /// </summary>
    /// <param name="ptr">Pointer to get the value from</param>
    /// <returns>The first value of the pointer</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the pointer length is 0</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T operator +(ConstPtr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : ptr._ptr;

    /// <summary>
    /// Determines whether the two pointers point to the same memory and have the same length. (This doesn't check the data itself)
    /// </summary>
    /// <param name="ptr1">Pointer to be compared with ptr2</param>
    /// <param name="ptr2">Pointer to be compared with ptr1</param>
    /// <returns>True if the two pointers point to the same memory and have the same length</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ConstPtr<T> ptr1, ConstPtr<T> ptr2) => ptr1._length == ptr2._length && Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    /// <summary>
    /// Determines whether the two pointers don't point to the same memory or don't have the same length (This doesn't check the data itself)
    /// </summary>
    /// <param name="ptr1">Pointer to be compared with ptr2</param>
    /// <param name="ptr2">Pointer to be compared with ptr1</param>
    /// <returns>True if the two pointers don't point to the same memory or don't have the same length, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ConstPtr<T> ptr1, ConstPtr<T> ptr2) => ptr1._length != ptr2._length || !Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    /// <summary>
    /// Enumerates the data pointed to by this pointer
    /// </summary>
    /// <returns>Enumerator that enumerates all the data in the memory pointed to by this pointer</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerates data pointed to by a ConstPtr
    /// </summary>
    public ref struct Enumerator
    {
        private ConstPtr<T> _ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ConstPtr<T> ptr)
        {
            _ptr = new(ref Unsafe.Add(ref ptr._ptr, -1), ptr._length + 1);
        }

        /// <summary>
        /// Moves to the next position
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => (++_ptr)._length != 0;

        /// <summary>
        /// Gets the current item
        /// </summary>
        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ptr.Value;
        }
    }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

    /// <summary>
    /// Always throws exception. Use the == operator instead
    /// </summary>
    /// <param name="obj">Doesn't matter</param>
    /// <returns>Nothing</returns>
    /// <exception cref="InvalidOperationException">Thrown always</exception>
    [Obsolete("ConstPtr.Equals will always throw exception, use the == operator instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new InvalidOperationException("Cannot use Equals on type ConstPtr, use the == operator instead");

    /// <summary>
    /// Always throw exceptrion.
    /// </summary>
    /// <returns>Nothing</returns>
    /// <exception cref="InvalidOperationException">Thrown always</exception>
    [Obsolete("ConstPtr.GetHashCode will always throw exception")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new InvalidOperationException("Cannot use GetHashCode on type ConstPtr");

#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
}
