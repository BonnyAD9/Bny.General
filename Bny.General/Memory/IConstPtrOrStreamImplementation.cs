namespace Bny.General.Memory;

internal interface IConstPtrOrStreamImplementation
{
    public ConstPtr<byte> Read(ConstPtrOrStream cpos, int length);
    public int ReadTo(ConstPtrOrStream cpos, Ptr<byte> result);
    public ConstPtr<byte> ReadAll(ConstPtrOrStream cpos);
    public byte[] GetAll(ConstPtrOrStream cpos);
    public int Seek(ConstPtrOrStream cpos, int offset, SeekOrigin origin);
    public int GetPosition(ConstPtrOrStream cpos);
}
