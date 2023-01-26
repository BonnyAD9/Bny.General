using Bny.General.Memory;

namespace Bny.General.Tester.Memory;

[UnitTest]
internal class PtrOrStreamTests
{
    [UnitTest]
    public static void Test_Constructors(Asserter a)
    {
        PtrOrStream pos = new(new byte[5]);

        a.Assert(!pos.IsStream);
        a.Assert(pos.CanRead);
        a.Assert(pos.CanSeek);

        pos = new(new MemoryStream());

        a.Assert(pos.IsStream);
        a.Assert(pos.CanRead);
        a.Assert(pos.CanSeek);
        a.Assert(pos.CanWrite);

        pos = new(new MemoryStream(new byte[5], false));

        a.Assert(pos.IsStream);
        a.Assert(pos.CanRead);
        a.Assert(pos.CanSeek);
        a.Assert(!pos.CanWrite);
    }

    [UnitTest]
    public static void Test_Write_Span(Asserter a)
    {
        var arr = new byte[5];
        PtrOrStream pos = arr;

        var p = pos.StartWrite(3);
        ((Span<byte>)p).Fill(2);
        pos.EndWrite(p);

        a.Assert(((Span<byte>)arr).StartsWith(new byte[] { 2, 2, 2, 0, 0 }));

        pos.Seek(-1, SeekOrigin.Current);

        p = pos.StartReadWrite(3);
        a.Assert(p.Length == 3);
        a.Assert(((Span<byte>)p).StartsWith(new byte[] { 2, 0, 0 }));

        p[0] = 3;
        p[2] = 3;
        pos.EndReadWrite(p);

        a.Assert(((Span<byte>)arr).StartsWith(new byte[] { 2, 2, 3, 0, 3 }));

        pos.Seek(1, SeekOrigin.Begin);

        pos.WriteFrom(arr[2..]);

        a.Assert(((Span<byte>)arr).StartsWith(new byte[] { 2, 3, 0, 3, 3 }));
    }

    [UnitTest]
    public static void Test_Write_Stream(Asserter a)
    {
        var arr = new byte[5];
        PtrOrStream pos = new MemoryStream(arr);

        var p = pos.StartWrite(3);
        ((Span<byte>)p).Fill(2);
        pos.EndWrite(p);

        a.Assert(((Span<byte>)arr).StartsWith(new byte[] { 2, 2, 2, 0, 0 }));

        pos.Seek(-1, SeekOrigin.Current);

        p = pos.StartReadWrite(3);
        a.Assert(p.Length == 3);
        a.Assert(((Span<byte>)p).StartsWith(new byte[] { 2, 0, 0 }));

        p[0] = 3;
        p[2] = 3;
        pos.EndReadWrite(p);

        a.Assert(((Span<byte>)arr).StartsWith(new byte[] { 2, 2, 3, 0, 3 }));

        pos.Seek(1, SeekOrigin.Begin);

        pos.WriteFrom(arr[2..]);

        a.Assert(((Span<byte>)arr).StartsWith(new byte[] { 2, 3, 0, 3, 3 }));
    }

    [UnitTest]
    public static void Test_ConversionsTo(Asserter a)
    {
        var arr = new byte[] { 9, 8, 7, 6, 5 };

        PtrOrStream cpos = (PtrOrStream)arr;
        var p = cpos.Read(10);
        a.Assert(p.Length == 5);
        a.Assert(p.StartsWith(arr));

        cpos = new MemoryStream(arr);
        p = cpos.Read(10);
        a.Assert(p.Length == 5);
        a.Assert(p.StartsWith(arr));

        cpos = arr;
        p = cpos.Read(10);
        a.Assert(p.Length == 5);
        a.Assert(p.StartsWith(arr));

        cpos = (Span<byte>)arr;
        p = cpos.Read(10);
        a.Assert(p.Length == 5);
        a.Assert(p.StartsWith(arr));
    }

    [UnitTest]
    public static void Test_ConversionsFrom(Asserter a)
    {
        var arr = new byte[] { 9, 8, 7, 6, 5 };
        var ms = new MemoryStream(arr);

        PtrOrStream cpos = ms;
        a.Assert(ReferenceEquals(ms, (Stream)cpos));

        cpos = arr;
        a.Assert((Ptr<byte>)arr == (Ptr<byte>)cpos);
    }
}
