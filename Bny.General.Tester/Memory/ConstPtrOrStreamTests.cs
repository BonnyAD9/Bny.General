using Bny.General.Memory;

namespace Bny.General.Tester.Memory;

[UnitTest]
internal class ConstPtrOrStreamTests
{
    [UnitTest]
    public static void Test_Constructors(Asserter a)
    {
        ConstPtrOrStream cpos = new(new byte[5]);

        a.Assert(!cpos.IsStream);
        a.Assert(cpos.CanRead);
        a.Assert(cpos.CanSeek);

        var ms = new MemoryStream(new byte[5]);
        ms.ReadByte();
        cpos = new(ms);

        a.Assert(cpos.IsStream);
        a.Assert(cpos.CanRead);
        a.Assert(cpos.CanSeek);
        a.Assert(cpos.Position == 1);

        cpos = new(ms, false);

        a.Assert(cpos.IsStream);
        a.Assert(cpos.CanRead);
        a.Assert(cpos.CanSeek);
        a.Assert(cpos.Position == 0);
    }

    [UnitTest]
    public static void Test_Methods_Span(Asserter a)
    {
        Ptr<byte> arr = new byte[] { 9, 8, 7, 6, 5 };
        ConstPtrOrStream cpos = new(arr);

        var p = cpos.Read(2);
        a.Assert(p.Length == 2);
        a.Assert(p[0] == 9 && p[1] == 8);

        var b = new byte[2];
        a.Assert(cpos.ReadTo(b) == 2);
        a.Assert(b[0] == 7 && b[1] == 6);

        a.Assert(cpos.Seek(-3, SeekOrigin.Current) == 1);

        p = cpos.ReadAll();
        a.Assert(p.Length == 4);
        a.Assert(p.StartsWith(arr[1..]));

        cpos.Seek(0, SeekOrigin.Begin);

        a.Assert(arr.HasSameContents(cpos.GetAll()));
    }

    [UnitTest]
    public static void Test_Methods_Stream(Asserter a)
    {
        var arr = new byte[] { 9, 8, 7, 6, 5 };
        ConstPtrOrStream cpos = new(new MemoryStream(arr, false));

        var p = cpos.Read(2);
        a.Assert(p.Length == 2);
        a.Assert(p[0] == 9 && p[1] == 8);

        var b = new byte[2];
        a.Assert(cpos.ReadTo(b) == 2);
        a.Assert(b[0] == 7 && b[1] == 6);

        a.Assert(cpos.Seek(-3, SeekOrigin.Current) == 1);

        p = cpos.ReadAll();
        a.Assert(p.Length == 4);
        a.Assert(p.StartsWith(arr[1..]));

        cpos.Seek(0, SeekOrigin.Begin);

        a.Assert(((Ptr<byte>)arr).HasSameContents(cpos.GetAll()));
    }

    [UnitTest]
    public static void Test_ConversionsTo(Asserter a)
    {
        var arr = new byte[] { 9, 8, 7, 6, 5 };

        ConstPtrOrStream cpos = (PtrOrStream)arr;
        var p = cpos.Read(10);
        a.Assert(p.Length == 5);
        a.Assert(p.StartsWith(arr));

        cpos = (ConstPtr<byte>)arr;
        p = cpos.Read(10);
        a.Assert(p.Length == 5);
        a.Assert(p.StartsWith(arr));

        cpos = (Ptr<byte>)arr;
        p = cpos.Read(10);
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

        cpos = (ReadOnlySpan<byte>)arr;
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

        ConstPtrOrStream cpos = ms;
        a.Assert(ReferenceEquals(ms, (Stream)cpos));

        cpos = arr;
        a.Assert((ConstPtr<byte>)arr == (ConstPtr<byte>)cpos);
    }
}
