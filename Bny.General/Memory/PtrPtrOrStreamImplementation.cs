using System.Diagnostics;

namespace Bny.General.Memory;

internal class PtrPtrOrStreamImplementation : IPtrOrStreamImplementation
{
    private int _offset;

    public ConstPtr<byte> Read(ConstPtrOrStream cpos, int length)
    {
        length = Math.Min(length, cpos._ptr.Length - _offset);
        _offset += length;
        return cpos._ptr[(_offset - length).._offset];
    }

    public int ReadTo(ConstPtrOrStream cpos, Ptr<byte> result)
    {
        ReadOnlySpan<byte> data = Read(cpos, result.Length);
        data.CopyTo(result);
        return data.Length;
    }

    public ConstPtr<byte> ReadAll(ConstPtrOrStream cpos)
        => cpos._ptr[_offset..];

    public byte[] GetAll(ConstPtrOrStream cpos) => ReadAll(cpos).ToArray();

    public int Seek(ConstPtrOrStream cpos, int offset, SeekOrigin origin)
    {
        var newPos = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _offset + offset,
            SeekOrigin.End => cpos._ptr.Length + offset,
            _ => throw new UnreachableException()
        };

        return _offset = Math.Clamp(newPos, 0, cpos._ptr.Length);
    }

    public int GetPosition(ConstPtrOrStream cpos) => _offset;

    public Ptr<byte> StartWrite(PtrOrStream pos, int length)
    {
        length = Math.Min(length, pos._ptr.Length - _offset);
        _offset += length;
        return pos._ptr[(_offset - length).._offset];
    }

    public void EndWrite(PtrOrStream pos, Ptr<byte> ptr) { }

    public Ptr<byte> StartReadWrite(PtrOrStream pos, int length)
        => StartWrite(pos, length);

    public void EndReadWrite(PtrOrStream pos, Ptr<byte> ptr) { }

    public int WriteFrom(PtrOrStream pos, ConstPtr<byte> source)
    {
        ReadOnlySpan<byte> data =
            source[..Math.Min(pos._ptr.Length - _offset, source.Length)];
        data.CopyTo(pos._ptr[_offset..]);
        return data.Length;
    }
}
