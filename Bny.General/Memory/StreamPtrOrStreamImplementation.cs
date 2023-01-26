namespace Bny.General.Memory;

internal class StreamPtrOrStreamImplementation
    : IPtrOrStreamImplementation
{
    public ConstPtr<byte> Read(ConstPtrOrStream cpos, int length)
    {
        byte[] buf = new byte[length];
        int rb = cpos._stream.Read(buf);
        return ((ConstPtr<byte>)buf)[..rb];
    }

    public int ReadTo(ConstPtrOrStream cpos, Ptr<byte> output)
        => cpos._stream.Read(output);

    public void Seek(ConstPtrOrStream cpos, int offset, SeekOrigin origin)
        => cpos._stream.Seek(offset, origin);

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
