using System.Runtime.CompilerServices;

namespace Bny.General.Memory;

/// <summary>
/// Represents either read only memory or read only stream
/// </summary>
public readonly ref struct ConstPtrOrStream
{
    internal readonly ConstPtr<byte> _ptr;
    internal readonly Stream _stream;

    private readonly IConstPtrOrStreamImplementation _implementation;

    /// <summary>
    /// Current position in the stream or span
    /// </summary>
    public int Position => _implementation.GetPosition(this);

    /// <summary>
    /// Shows whether this is stream
    /// </summary>
    public bool IsStream => _stream is not null;

    /// <summary>
    /// If this is stream, determines whether it can read. True for ptr
    /// </summary>
    public bool CanRead => _stream is null || _stream.CanRead;

    /// <summary>
    /// If this is, stream determines whether it can seek. True for ptr
    /// </summary>
    public bool CanSeek => _stream is null || _stream.CanSeek;

    /// <summary>
    /// Creates the ptr version
    /// </summary>
    /// <param name="ptr">backing pointer</param>
    public ConstPtrOrStream(ConstPtr<byte> ptr)
    {
        _ptr = ptr;
        _stream = default!;
        _implementation = new PtrPtrOrStreamImplementation();
    }

    /// <summary>
    /// Creates the Stream version (inherits position)
    /// </summary>
    /// <param name="stream">backing stream</param>
    public ConstPtrOrStream(Stream stream) : this(stream, true) { }

    /// <summary>
    /// Creates the stream version
    /// </summary>
    /// <param name="stream">backing stream</param>
    /// <param name="inheritPos">
    /// True if the position should be inherited from stream
    /// (if true seeking to the beggining will seek to the begining of the
    /// stream, if false seeking to the beggining will seek to the position
    /// in the stream at the time it was given)
    /// </param>
    public ConstPtrOrStream(Stream stream, bool inheritPos)
    {
        _stream = stream;
        _ptr = default;
        _implementation = new StreamPtrOrStreamImplementation(this, inheritPos);
    }

    /// <summary>
    /// DON'T USE THIS CONSTRUCTOR, THIS WILL BE UNINITALIZED
    /// </summary>
    [Obsolete(
        "Don't use this constructor, it will create uninitalized " +
        "ConstPtrOrStream instance. If that is your intention use the " +
        "'default' keyword")]
    public ConstPtrOrStream()
    {
        _stream = default!;
        _ptr = default;
        _implementation = default!;
    }

    /// <summary>
    /// Reads data from the span or stream and moves the position
    /// </summary>
    /// <param name="length">Size of the data to read</param>
    /// <returns>
    /// Pointer to the readed data, its length may be less than was requested
    /// if there is no more data
    /// </returns>
    public ConstPtr<byte> Read(int length)
        => _implementation.Read(this, length);

    /// <summary>
    /// Reads data into the given ptr
    /// </summary>
    /// <param name="result">Where to read the data to</param>
    /// <returns>Numbe of bytes written to result</returns>
    public int ReadTo(Ptr<byte> result)
        => _implementation.ReadTo(this, result);

    /// <summary>
    /// Reads all the remaining data
    /// </summary>
    /// <returns>Pointer to the data</returns>
    public ConstPtr<byte> ReadAll() => _implementation.ReadAll(this);

    /// <summary>
    /// Gets all the remaining data. If you don't need the mutable array
    /// use ReadAll instead.
    /// </summary>
    /// <returns>All the data to the end of the stream</returns>
    public byte[] GetAll() => _implementation.GetAll(this);

    /// <summary>
    /// Seeks in the stream or ptr
    /// </summary>
    /// <param name="offset">How much to seek</param>
    /// <param name="origin">Where to seek from</param>
    /// <returns>The new position</returns>
    public int Seek(int offset, SeekOrigin origin)
        => _implementation.Seek(this, offset, origin);

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(PtrOrStream pos)
        => pos._stream is null ? new(pos._ptr) : new(pos._stream);

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(ConstPtr<byte> ptr)
        => new(ptr);

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(Ptr<byte> ptr)
        => new(ptr);

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(Stream s) => new(s);

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(byte[] arr) => new(arr);

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(ReadOnlySpan<byte> arr)
        => new(arr);

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(Span<byte> arr)
        => new(arr);

    /// <summary>
    /// Gets the ptr if this is not stream, otherwise throws
    /// </summary>
    /// <param name="cpos"></param>
    public static explicit operator ConstPtr<byte>(ConstPtrOrStream cpos)
        => cpos.IsStream
            ? throw new InvalidOperationException(
                "This ConstPtrOrStream is stream")
            : cpos._ptr;

    /// <summary>
    /// Gets the Stream if this is stream, otherwise throws
    /// </summary>
    /// <param name="cpos"></param>
    public static explicit operator Stream(ConstPtrOrStream cpos)
        => cpos.IsStream
            ? cpos._stream
            : throw new InvalidOperationException(
                "This ConstPtrOrStream is Ptr");
}
