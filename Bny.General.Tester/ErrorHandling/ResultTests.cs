using Bny.General.ErrorHandling;
using System.Security.Cryptography.X509Certificates;

namespace Bny.General.Tester.ErrorHandling;

[UnitTest]
internal class ResultTests
{
    [UnitTest]
    public static void Test_Constructors(Asserter a)
    {
        var value = Random.Shared.Next(2) == 1;

        Result r = new(value, "myMessage");
        a.Assert(r.Success == value);
        a.Assert(r.Message == "myMessage");

        r = new(value);
        a.Assert(r.Success == value);
        a.Assert(r.Message is null);

        r = new("myMessage");
        a.Assert(r.Failed);
        a.Assert(r.Message == "myMessage");

        r = new();
        a.Assert(r.Success);
        a.Assert(r.Message is null);
    }

    [UnitTest]
    public static void Test_Throw(Asserter a)
    {
        Result r = new();
        bool didThrow = false;

        try
        {
            r.ThrowOnFail();
        }
        catch
        {
            didThrow = true;
        }

        a.Assert(!didThrow);

        r = new("myMessage");
        didThrow = false;

        try
        {
            r.ThrowOnFail();
        }
        catch (Exception e)
        {
            didThrow = true;
            a.Assert(e.Message == "myMessage");
        }

        a.Assert(didThrow);
    }

    [UnitTest]
    public static void Test_Operators(Asserter a)
    {
        Result s = new();
        Result f = new("myMessage");

        a.Assert((bool)s);
        a.Assert(!(bool)f);

        a.Assert(! !s);
        a.Assert(!f);

        bool st = true;
        _ = s || (st = false);
        a.Assert(st);

        bool ff = true;
        _ = f && (ff = false);
        a.Assert(ff);
    }
}
