using Bny.General.ErrorHandling;

namespace Bny.General.Tester.ErrorHandling;

[UnitTest]
internal class Result_T_Test
{
    [UnitTest]
    public static void Test_Constructors(Asserter a)
    {
        var value = Random.Shared.Next();
        var suc = Random.Shared.Next(2) == 1;
        var msg = Random.Shared.Next().ToString();

        Result<int> r = new(value, suc, msg);
        a.Assert(r.Value == value);
        a.Assert(r.Success == suc);
        a.Assert(r.Message == msg);

        r = new(value, suc);
        a.Assert(r.Value == value);
        a.Assert(r.Success == suc);
        a.Assert(r.Message is null);

        r = new(value, msg);
        a.Assert(r.Value == value);
        a.Assert(r.Failed);
        a.Assert(r.Message == msg);

        r = new(msg);
        a.Assert(r.Value == default);
        a.Assert(r.Failed);
        a.Assert(r.Message == msg);

        r = new(value);
        a.Assert(r.Value == value);
        a.Assert(r.Success);
        a.Assert(r.Message is null);
    }

    [UnitTest]
    public static void Test_GetOrThrow(Asserter a)
    {
        var value = Random.Shared.Next();
        var msg = Random.Shared.Next().ToString();

        Result<int> r = new(value);
        bool didThrow = false;

        try
        {
            a.Assert(r.GetOrThrow() == value);
        }
        catch
        {
            didThrow = true;
        }

        a.Assert(!didThrow);

        r = new(msg);
        didThrow = false;

        try
        {
            r.GetOrThrow();
        }
        catch (Exception ex)
        {
            didThrow = true;
            a.Assert(ex.Message == msg);
        }

        a.Assert(didThrow);
    }

    [UnitTest]
    public static void TestEqualities(Asserter a)
    {
        var same = Random.Shared.Next();
        var diff = same + 1;

        Result<int> s = new(same);
        Result<int> f = new(same, false);

        a.Assert(f.FailedAndEqual(same));
        a.Assert(f.FailedAndNotEqual(diff));
        a.Assert(s == same);
        a.Assert(s != diff);

        a.Assert(!s.FailedAndEqual(same));
        a.Assert(!s.FailedAndNotEqual(diff));
        a.Assert(!(f == same));
        a.Assert(!(f != diff));

        Result<int> sd = new(diff);
        Result<int> s2 = s;

        a.Assert(s == s2);
        a.Assert(s != f);
        a.Assert(s != sd);
    }

    [UnitTest]
    public static void Test_Conversions(Asserter a)
    {
        var value = Random.Shared.Next();
        var msg = Random.Shared.Next().ToString();

        Result<int> r = value;

        a.Assert(r == value);

        bool didThrow = false;
        try
        {
            a.Assert((int)r == value);
        }
        catch
        {
            didThrow = true;
        }
        a.Assert(!didThrow);

        r = new(msg);
        didThrow = false;
        try
        {
            _ = (int)r;
        }
        catch (Exception ex)
        {
            didThrow = true;
            a.Assert(ex.Message == msg);
        }
        a.Assert(didThrow);
    }
}
