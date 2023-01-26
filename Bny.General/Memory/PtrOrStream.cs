using System.Runtime.CompilerServices;

namespace Bny.General.Memory;

/// <summary>
/// Represents readable and writable stream or ptr
/// </summary>
public readonly ref struct PtrOrStream
{
    internal readonly Ptr<byte> _ptr;
    internal readonly Stream _stream;

    private readonly IPtrOrStreamImplementation _implementation;

    /// <summary>
    /// Shows whether this is stream
    /// </summary>
    public bool IsStream => _stream is not null;

    /// <summary>
    /// If this is stream, determines whether it can read. True for ptr
    /// </summary>
    public bool CanRead => _stream is null || _stream.CanRead;

    /// <summary>
    /// If this is stream, determines whether it can seek. True for ptr
    /// </summary>
    public bool CanSeek => _stream is null || _stream.CanSeek;

    /// <summary>
    /// If this is stream, determines whether it can write. True for ptr
    /// </summary>
    public bool CanWrite => _stream is null || _stream.CanWrite;

    /// <summary>
    /// Creates the ptr version
    /// </summary>
    /// <param name="ptr">backing pointer</param>
    public PtrOrStream(Ptr<byte> ptr)
    {
        _ptr = ptr;
        _stream = null!;
        _implementation = new PtrPtrOrStreamImplementation();
    }

    /// <summary>
    /// Creates the Stream version
    /// </summary>
    /// <param name="stream">backing stream</param>
    public PtrOrStream(Stream stream)
    {
        _stream = stream;
        _ptr = new(ref Unsafe.NullRef<byte>(), 0);
        _implementation = new StreamPtrOrStreamImplementation();
    }

    /// <summary>
    /// DON'T USE THIS CONSTRUCTOR, THIS WILL BE UNINITALIZED
    /// </summary>
    [Obsolete(
        "Don't use this constructor, it will create uninitalized " +
        "PtrOrStream instance. If that is your intention use the 'default' " +
        "keyword")]
    public PtrOrStream()
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
    /// Seeks in the stream or ptr
    /// </summary>
    /// <param name="offset">How much to seek</param>
    /// <param name="origin">Where to seek from</param>
    public void Seek(int offset, SeekOrigin origin)
        => _implementation.Seek(this, offset, origin);

    /// <summary>
    /// Gets memory where to write and begins the write
    /// </summary>
    /// <param name="length">Size of memory to get</param>
    /// <returns>
    /// Memory where to write, its size may be smaller than requested
    /// </returns>
    public Ptr<byte> StartWrite(int length)
        => _implementation.StartWrite(this, length);

    /// <summary>
    /// Ends the writing to the memory. Must be called in the same order as
    /// StartWrite
    /// </summary>
    /// <param name="ptr">
    /// Memory to write that was given by the StartWrite method
    /// </param>
    public void EndWrite(Ptr<byte> ptr) =>
        _implementation.EndWrite(this, ptr);

    /// <summary>
    /// Gets memory with data from the stream or ptr. If this is stream it
    /// must be seekable for this to work.
    /// </summary>
    /// <param name="length">Size of the data to get</param>
    /// <returns>
    /// Memory where to write with original data, its size may be smaller than
    /// requested
    /// </returns>
    public Ptr<byte> StartReadWrite(int length)
        => _implementation.StartReadWrite(this, length);

    /// <summary>
    /// Ends the writing to the memory. Must be called in the same order as
    /// StartReadWrite
    /// </summary>
    /// <param name="ptr">
    /// Memory to write that was given by the StartReadWrite method
    /// </param>
    public void EndReadWrite(Ptr<byte> ptr)
        => _implementation.EndReadWrite(this, ptr);

    /// <summary>
    /// Writes the given data to the stream or ptr
    /// </summary>
    /// <param name="source">Data to write</param>
    /// <returns>Number of written bytes</returns>
    public int WriteFrom(ConstPtr<byte> source)
        => _implementation.WriteFrom(this, source);

    /// <inheritdoc/>
    public static implicit operator PtrOrStream(Ptr<byte> ptr) => new(ptr);

    /// <inheritdoc/>
    public static implicit operator PtrOrStream(Stream s) => new(s);

    /// <inheritdoc/>
    public static implicit operator PtrOrStream(byte[] arr) => new(arr);

    /// <inheritdoc/>
    public static implicit operator PtrOrStream(Span<byte> arr) => new(arr);

    /// <summary>
    /// Gets the ptr if this is not stream, otherwise throws
    /// </summary>
    /// <param name="pos"></param>
    public static explicit operator Ptr<byte>(PtrOrStream pos) => pos.IsStream
        ? throw new InvalidOperationException("This ConstPtrOrStream is stream")
        : pos._ptr;

    /// <summary>
    /// Gets the Stream if this is stream, otherwise throws
    /// </summary>
    /// <param name="pos"></param>
    public static explicit operator Stream(PtrOrStream pos) => pos.IsStream
        ? pos._stream
        : throw new InvalidOperationException("This ConstPtrOrStream is Ptr");
}
