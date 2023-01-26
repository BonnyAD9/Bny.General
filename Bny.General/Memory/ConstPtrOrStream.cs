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
    /// Creates the ptr version
    /// </summary>
    /// <param name="ptr">backing pointer</param>
    public ConstPtrOrStream(ConstPtr<byte> ptr)
    {
        _ptr = ptr;
        _stream = null!;
        _implementation = new PtrPtrOrStreamImplementation();
    }

    /// <summary>
    /// Creates the Stream version
    /// </summary>
    /// <param name="stream">backing stream</param>
    public ConstPtrOrStream(Stream stream)
    {
        _stream = stream;
        _ptr = new(ref Unsafe.NullRef<byte>(), 0);
        _implementation = new StreamPtrOrStreamImplementation();
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

    /// <inheritdoc/>
    public static implicit operator ConstPtrOrStream(PtrOrStream pos)
        => pos._stream is null ? new(pos._ptr) : new(pos._stream);
}
