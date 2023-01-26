namespace Bny.General.Memory;

internal interface IPtrOrStreamImplementation
    : IConstPtrOrStreamImplementation
{
    public Ptr<byte> StartWrite(PtrOrStream pos, int length);
    public void EndWrite(PtrOrStream pos, Ptr<byte> ptr);
    public Ptr<byte> StartReadWrite(PtrOrStream pos, int length);
    public void EndReadWrite(PtrOrStream pos, Ptr<byte> ptr);
    public int WriteFrom(PtrOrStream pos, ConstPtr<byte> source);
}
