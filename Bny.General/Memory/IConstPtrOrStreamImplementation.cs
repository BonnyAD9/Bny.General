namespace Bny.General.Memory;

internal interface IConstPtrOrStreamImplementation
{
    public ConstPtr<byte> Read(ConstPtrOrStream cpos, int length);
    public int ReadTo(ConstPtrOrStream cpos, Ptr<byte> result);
    public void Seek(ConstPtrOrStream cpos, int offset, SeekOrigin origin);
}
