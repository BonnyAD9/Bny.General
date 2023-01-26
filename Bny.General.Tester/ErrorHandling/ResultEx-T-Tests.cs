using Bny.General.ErrorHandling;
using Ex = System.NotImplementedException;

namespace Bny.General.Tester.ErrorHandling;

[UnitTest]
internal class ResultEx_T_Tests
{
    [UnitTest]
    public static void Test_Constructors(Asserter a)
    {
        var value = Random.Shared.Next();
        var msg = Random.Shared.Next().ToString();

        Result<int, Ex> r = new(value, false, msg);
        a.Assert(r.Value == value);
        TestThrowIs(a, r, msg);

        r = new(value, false);
        a.Assert(r.Value == value);
        TestThrowIs(a, r, new Ex().Message);

        r = new(value, msg);
        a.Assert(r.Value == value);
        TestThrowIs(a, r, msg);

        r = new(msg);
        a.Assert(r.Value == default);
        TestThrowIs(a, r, msg);

        r = new(value);
        a.Assert(r.Value == value);
        a.Assert(r.Success);
        a.Assert(r.Message is null);
    }

    private static void TestThrowIs<T, TEx>(
        Asserter a, Result<T, TEx> res, string? msg)
        where TEx : Exception
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
            a.Assert(ex is TEx);
        }

        a.Assert(didThrow);
    }

    [UnitTest]
    public static void Test_Conversions(Asserter a)
    {
        var value = Random.Shared.Next();

        Result<int, Ex> r = value;

        a.Assert(r.Value == value);
        a.Assert(r.Success);
        a.Assert(r.Message is null);
    }
}
