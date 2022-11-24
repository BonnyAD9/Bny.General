using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bny.General;

public readonly ref struct Ptr<T>
{
    internal readonly ref T _ptr;
    internal readonly int _length;

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

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

    public T this[int index]
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (uint)index < (uint)_length ? At(index) : throw new IndexOutOfRangeException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => At(index) = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(ref T ptr, int length)
    {
        if (length < 0)
            throw new IndexOutOfRangeException();
        _length = length;
        _ptr = ref ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(ReadOnlySpan<T> span) : this(ref MemoryMarshal.GetReference(span), span.Length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(Span<T> span) : this(ref MemoryMarshal.GetReference(span), span.Length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(T[] arr) : this(ref MemoryMarshal.GetArrayDataReference(arr), arr.Length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref T At(int index) => ref Unsafe.Add(ref _ptr, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr<T> Slice(int start, int length) => (uint)start >= (uint)_length || (uint)(start + length) > (uint)_length
        ? throw new IndexOutOfRangeException()
        : new(ref At(start), length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(Ptr<T> ptr) => MemoryMarshal.CreateSpan(ref ptr._ptr, ptr._length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Ptr<T>(Span<T> span) => new(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Ptr<T>(T[] arr) => new(arr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ptr<T> operator ++(Ptr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : new(ref ptr.At(1), ptr._length - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Ptr<T> ptr) => ptr.Length != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Ptr<T> ptr) => ptr.Length == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(Ptr<T> ptr) => ptr.Length == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ptr<T> operator +(Ptr<T> ptr, int value) => (uint)value < (uint)ptr._length ? new(ref ptr.At(value), ptr._length - value) : throw new IndexOutOfRangeException();

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(Ptr<T> p1, Ptr<T> p2) => (int)Unsafe.ByteOffset(ref p1._ptr, ref p2._ptr) / sizeof(T);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(Ptr<T> p1, ConstPtr<T> p2) => (int)Unsafe.ByteOffset(ref p1._ptr, ref p2._ptr) / sizeof(T);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int operator -(ConstPtr<T> p1, Ptr<T> p2) => (int)Unsafe.ByteOffset(ref p1._ptr, ref p2._ptr) / sizeof(T);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T operator +(Ptr<T> ptr) => ptr._length == 0 ? throw new IndexOutOfRangeException() : ptr._ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    public ref struct Enumerator
    {
        private Ptr<T> _ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Ptr<T> ptr)
        {
            _ptr = new(ref Unsafe.Add(ref ptr._ptr, -1), ptr._length + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => (++_ptr)._length != 0;

        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _ptr.Value;
        }
    }
}
