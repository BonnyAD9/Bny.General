using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bny.General.Memory;

/// <summary>
/// Mutable version of ConstPtr. Represents continuous memory. Similar to Span, but has pointer arithmetic operator overloads.
/// </summary>
/// <typeparam name="T">Type of the data pointed to</typeparam>
public readonly ref struct Ptr<T>
{
    internal readonly ref T _ptr;
    internal readonly int _length;

    /// <summary>
    /// Amount of elements in the memory
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <summary>
    /// Gets reference to the first element in the memory
    /// </summary>
    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_length == 0)
                throw new IndexOutOfRangeException();
            return ref _ptr;
        }
    }

    /// <summary>
    /// Gets/sets the memory at the given index 
    /// </summary>
    /// <param name="index">Index of the memory to access</param>
    /// <returns>Memory at the given index</returns>
    /// <exception cref="IndexOutOfRangeException">Throw when the index is larger than the length</exception>
    public ref T this[int index]
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index < (uint)_length)
                return ref At(index);
            throw new IndexOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Ptr(ref T ptr, int length)
    {
        if (length < 0)
            throw new IndexOutOfRangeException();
        _length = length;
        _ptr = ref ptr;
    }

    /// <summary>
    /// Creates the pointer from a Span
    /// </summary>
    /// <param name="span">Span to create the pointer from</param>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(Span<T> span) : this(ref MemoryMarshal.GetReference(span), span.Length) { }

    /// <summary>
    /// Craetes pointer from array
    /// </summary>
    /// <param name="arr">Array to create the pointer from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(T[] arr) : this(ref MemoryMarshal.GetArrayDataReference(arr), arr.Length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref T At(int index) => ref Unsafe.Add(ref _ptr, index);

    /// <summary>
    /// Gets slice of the memory pointed to by this
    /// </summary>
    /// <param name="start">Index of the slice to make</param>
    /// <param name="length">Number of elements to be in the slice</param>
    /// <returns>New pointer over the memory slice</returns>
    /// <exception cref="IndexOutOfRangeException">Throw when the slice is out of range</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr<T> Slice(int start, int length) => (uint)start >= (uint)_length || (uint)(start + length) > (uint)_length
        ? throw new IndexOutOfRangeException()
        : new(ref At(start), length);

    /// <summary>
    /// Returns reference to the first item in the memory
    /// </summary>
    /// <returns>Reference to the firs item in the memory</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetPinnableReference() => ref _ptr;

    /// <summary>
    /// Converts pointer to ReadOnlySpan
    /// </summary>
    /// <param name="ptr">Pointer to convert to ReadOnlySpan</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(Ptr<T> ptr) => MemoryMarshal.CreateSpan(ref ptr._ptr, ptr._length);

    /// <summary>
    /// Converts pointer to Span
    /// </summary>
    /// <param name="ptr">Pointer to convert to span</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(Ptr<T> ptr) => MemoryMarshal.CreateSpan(ref ptr._ptr, ptr._length);

    /// <summary>
    /// Convers Span to pointer (same as the Ptr(Span) constructor)
    /// </summary>
    /// <param name="span"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Ptr<T>(Span<T> span) => new(span);

    /// <summary>
    /// Converts array to pointer (same as the Ptr(Array) constructor)
    /// </summary>
    /// <param name="arr"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Ptr<T>(T[] arr) => new(arr);

    /// <summary>
    /// Increments the pointer ('++ptr' has the same effect as 'ptr = ptr[1..]')
    /// </summary>
    /// <param name="ptr">Pointer to increment</param>
    /// <returns>Incremented pointer</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the pointer has length of 0</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ptr<T> operator ++(Ptr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : new(ref ptr.At(1), ptr._length - 1);

    /// <summary>
    /// Determines whether the pointer points to non empty block of memory (has the same effect as ptr.Length != 0)
    /// </summary>
    /// <param name="ptr">Pointer to check</param>
    /// <returns>True if the memory block pointed to by ptr is not empty, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Ptr<T> ptr) => ptr.Length != 0;

    /// <summary>
    /// Determines whether the pointer points to emty block of memory (has the same effect as ptr.Length == 0)
    /// </summary>
    /// <param name="ptr">Ptr to check</param>
    /// <returns>True if the pointer points to empty block of memory, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Ptr<T> ptr) => ptr.Length == 0;

    /// <summary>
    /// Determines whether the pointer points to emty block of memory (has the same effect as ptr.Length == 0)
    /// </summary>
    /// <param name="ptr">Ptr to check</param>
    /// <returns>True if the pointer points to empty block of memory, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(Ptr<T> ptr) => ptr.Length == 0;

    /// <summary>
    /// Adds the given value to the pointer ('ptr + n' has the same effect as 'ptr[n..]')
    /// </summary>
    /// <param name="ptr">Pointer to add the value to</param>
    /// <param name="value">Value to be added to the pointer</param>
    /// <returns>Pointer shifted by the value</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the value is larger or equal to the pointer length</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ptr<T> operator +(Ptr<T> ptr, int value) => (uint)value < (uint)ptr._length ? new(ref ptr.At(value), ptr._length - value) : throw new IndexOutOfRangeException();

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    /// <summary>
    /// Determines the distance between two pointers in the same memory block. If the pointers aren't in the same memory block, the behaviour is undefined.
    /// </summary>
    /// <param name="p1">The second pointer in the memory block</param>
    /// <param name="p2">The first pointer in the memory block</param>
    /// <returns>The distance betwheen the two pointers</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(Ptr<T> p1, Ptr<T> p2) => (int)Unsafe.ByteOffset(ref p2._ptr, ref p1._ptr) / sizeof(T);

    /// <summary>
    /// Determines the distance between two pointers in the same memory block. If the pointers aren't in the same memory block, the behaviour is undefined.
    /// </summary>
    /// <param name="p1">The second pointer in the memory block</param>
    /// <param name="p2">The first pointer in the memory block</param>
    /// <returns>The distance betwheen the two pointers</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(Ptr<T> p1, ConstPtr<T> p2) => (int)Unsafe.ByteOffset(ref p2._ptr, ref p1._ptr) / sizeof(T);

    /// <summary>
    /// Determines the distance between two pointers in the same memory block. If the pointers aren't in the same memory block, the behaviour is undefined.
    /// </summary>
    /// <param name="p1">The second pointer in the memory block</param>
    /// <param name="p2">The first pointer in the memory block</param>
    /// <returns>The distance betwheen the two pointers</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(ConstPtr<T> p1, Ptr<T> p2) => (int)Unsafe.ByteOffset(ref p2._ptr, ref p1._ptr) / sizeof(T);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

    /// <summary>
    /// Dereferences the pointer. Gets the value of the pointer at the first index. (Returns the same value as ptr.Value)
    /// </summary>
    /// <param name="ptr">Pointer to dereference</param>
    /// <returns>Value of the pointer</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the pointer length is 0</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T operator +(Ptr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : ptr._ptr;

    /// <summary>
    /// Determines whether the two pointers point to the same memory and have the same length
    /// </summary>
    /// <param name="ptr1">Pointer to be compared with ptr2</param>
    /// <param name="ptr2">Pointer to be compared with ptr1</param>
    /// <returns>True if the pointers point to the same memory and have the same length, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Ptr<T> ptr1, Ptr<T> ptr2) => ptr1._length == ptr2._length && Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    /// <summary>
    /// Determines whether the two pointers point to different memory or have different length
    /// </summary>
    /// <param name="ptr1">Pointer to be compared with ptr2</param>
    /// <param name="ptr2">Pointer to be compared with ptr1</param>
    /// <returns>True if the pointers don't point to the same memory or don't have the same length</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Ptr<T> ptr1, Ptr<T> ptr2) => ptr1._length != ptr2._length || !Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    /// <summary>
    /// Determines whether the two pointers point to the same memory and have the same length
    /// </summary>
    /// <param name="ptr1">Pointer to be compared with ptr2</param>
    /// <param name="ptr2">Pointer to be compared with ptr1</param>
    /// <returns>True if the pointers point to the same memory and have the same length, otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ConstPtr<T> ptr1, Ptr<T> ptr2) => ptr1._length == ptr2._length && Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    /// <summary>
    /// Determines whether the two pointers point to different memory or have different length
    /// </summary>
    /// <param name="ptr1">Pointer to be compared with ptr2</param>
    /// <param name="ptr2">Pointer to be compared with ptr1</param>
    /// <returns>True if the pointers don't point to the same memory or don't have the same length</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ConstPtr<T> ptr1, Ptr<T> ptr2) => ptr1._length != ptr2._length || !Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    /// <summary>
    /// Enumerates all the values in the memory block pointed to by this
    /// </summary>
    /// <returns>Enumerator that enumerates all the values in the memory block pointed to by this</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerates all the value in the memory block pointed to by a Ptr
    /// </summary>
    public ref struct Enumerator
    {
        private Ptr<T> _ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Ptr<T> ptr)
        {
            _ptr = new(ref Unsafe.Add(ref ptr._ptr, -1), ptr._length + 1);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => (++_ptr)._length != 0;

        /// <inheritdoc/>
        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _ptr.Value;
        }
    }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

    /// <summary>
    /// Throw an exception. Use the == operator instead
    /// </summary>
    /// <param name="obj">Doesn't matter</param>
    /// <returns>Nothing</returns>
    /// <exception cref="InvalidOperationException">Thrown allways</exception>
    [Obsolete("ConstPtr.Equals will always throw exception, use the == operator instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new InvalidOperationException("Cannot use Equals on type ConstPtr, use the == operator instead");

    /// <summary>
    /// Throws an exception.
    /// </summary>
    /// <returns>Nothing</returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Obsolete("ConstPtr.GetHashCode will always throw exception")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new InvalidOperationException("Cannot use GetHashCode on type ConstPtr");

#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
}
