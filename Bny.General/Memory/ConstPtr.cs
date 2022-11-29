using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bny.General.Memory;

public readonly ref struct ConstPtr<T>
{
    internal readonly ref T _ptr;
    internal readonly int _length;

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    public T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => +this;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(ReadOnlySpan<T> span) : this(ref MemoryMarshal.GetReference(span), span.Length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(Span<T> span) : this(ref MemoryMarshal.GetReference(span), span.Length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(T[] arr) : this(ref MemoryMarshal.GetArrayDataReference(arr), arr.Length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr(Ptr<T> ptr) : this(ref ptr._ptr, ptr._length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref T At(int index) => ref Unsafe.Add(ref _ptr, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstPtr<T> Slice(int start, int length) => (uint)start >= (uint)_length || (uint)(start + length) > (uint)_length
        ? throw new IndexOutOfRangeException()
        : new(ref At(start), length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(Ptr<T> ptr) => new(ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(ConstPtr<T> ptr) => MemoryMarshal.CreateSpan(ref ptr._ptr, ptr._length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(ReadOnlySpan<T> span) => new(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(Span<T> span) => new(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ConstPtr<T>(T[] arr) => new(arr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstPtr<T> operator ++(ConstPtr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : new(ref ptr.At(1), ptr._length - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(ConstPtr<T> ptr) => ptr.Length != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(ConstPtr<T> ptr) => ptr.Length == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(ConstPtr<T> ptr) => ptr.Length == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstPtr<T> operator +(ConstPtr<T> ptr, int value) => (uint)value < (uint)ptr._length ? new(ref ptr.At(value), ptr._length - value) : throw new IndexOutOfRangeException();

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(ConstPtr<T> p1, ConstPtr<T> p2) => (int)Unsafe.ByteOffset(ref p1._ptr, ref p2._ptr) / sizeof(T);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T operator +(ConstPtr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : ptr._ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ConstPtr<T> ptr1, ConstPtr<T> ptr2) => ptr1._length == ptr2._length && Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ConstPtr<T> ptr1, ConstPtr<T> ptr2) => ptr1._length != ptr2._length || !Unsafe.AreSame(ref ptr1._ptr, ref ptr2._ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    public ref struct Enumerator
    {
        private ConstPtr<T> _ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ConstPtr<T> ptr)
        {
            _ptr = new(ref Unsafe.Add(ref ptr._ptr, -1), ptr._length + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => (++_ptr)._length != 0;

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ptr.Value;
        }
    }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

    [Obsolete("ConstPtr.Equals will always throw exception, use the == operator instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new InvalidOperationException("Cannot use Equals on type ConstPtr, use the == operator instead");

    [Obsolete("ConstPtr.GetHashCode will always throw exception")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new InvalidOperationException("Cannot use GetHashCode on type ConstPtr");

#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
}
