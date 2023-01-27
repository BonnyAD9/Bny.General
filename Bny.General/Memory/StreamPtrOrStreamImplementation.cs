namespace Bny.General.Memory;

internal class StreamPtrOrStreamImplementation
    : IPtrOrStreamImplementation
{
    private int _positionOffset;

    public StreamPtrOrStreamImplementation(ConstPtrOrStream cpos, bool inheritPos)
    {
        _positionOffset = inheritPos ? 0 : (int)cpos._stream.Position;
    }

    public ConstPtr<byte> Read(ConstPtrOrStream cpos, int length)
    {
        byte[] buf = new byte[length];
        int rb = cpos._stream.Read(buf);
        return ((ConstPtr<byte>)buf)[..rb];
    }

    public int ReadTo(ConstPtrOrStream cpos, Ptr<byte> output)
        => cpos._stream.Read(output);

    public ConstPtr<byte> ReadAll(ConstPtrOrStream cpos) => GetAll(cpos);

    public byte[] GetAll(ConstPtrOrStream cpos)
    {
        List<byte> buf = new();
        int b;
        while ((b = cpos._stream.ReadByte()) != -1)
            buf.Add((byte)b);
        return buf.ToArray();
    }

    public int Seek(ConstPtrOrStream cpos, int offset, SeekOrigin origin)
        => (int)cpos._stream.Seek(
            origin == SeekOrigin.Begin ? offset + _positionOffset : offset,
            origin) - _positionOffset;

    public int GetPosition(ConstPtrOrStream cpos)
        => (int)cpos._stream.Position - _positionOffset;

    public Ptr<byte> StartWrite(PtrOrStream pos, int length)
        => new byte[length];

    public void EndWrite(PtrOrStream pos, Ptr<byte> ptr)
        => WriteFrom(pos, ptr);

    public Ptr<byte> StartReadWrite(PtrOrStream pos, int length)
    {
        byte[] buf = new byte[length];
        int rb = pos._stream.Read(buf);
        var res = ((Ptr<byte>)buf)[..rb];
        Seek(pos, -rb, SeekOrigin.Current);
        return res;
    }

    public void EndReadWrite(PtrOrStream pos, Ptr<byte> ptr)
        => EndWrite(pos, ptr);

    public int WriteFrom(PtrOrStream pos, ConstPtr<byte> source)
    {
        pos._stream.Write(source);
        return source.Length;
    }
}
