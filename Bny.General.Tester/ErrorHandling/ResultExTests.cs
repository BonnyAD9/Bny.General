using Bny.General.ErrorHandling;
using Ex = System.NotImplementedException;

namespace Bny.General.Tester.ErrorHandling;

[UnitTest]
internal class ResultExTests
{
    [UnitTest]
    public static void Test_Constructors(Asserter a)
    {
        var msg = Random.Shared.Next().ToString();

        ResultEx<Ex> r = new(false, msg);
        TestThrowIs(a, r, msg);

        r = new(false);
        TestThrowIs(a, r, new Ex().Message);

        r = new(msg);
        TestThrowIs(a, r, msg);

        r = new();
        a.Assert(r.Success);
        a.Assert(r.Message is null);
    }

    private static void TestThrowIs<T>(
        Asserter a, ResultEx<T> res, string? msg)
        where T : Exception
    {
        bool didThrow = false;
        try
        {
            res.ThrowOnFail();
        }
        catch (Exception ex)
        {
            didThrow = true;
            a.Assert(ex.Message == msg);
            a.Assert(ex is T);
        }

        a.Assert(didThrow);
    }
}
