using Bny.General.Memory;
using Bny.UnitTests;

namespace Bny.General.Tester;

[UnitTest]
internal static class PtrTester
{
    [UnitTest]
    public static void Test_General(Asserter a)
    {
        int value = Random.Shared.Next(ushort.MaxValue);
        int[] arr = TestData.Generate(100, i => i + value);
        Span<int> span = new(arr);
        ConstPtr<int> cptr1 = span;

        Ptr<int> pArr = arr;
        Ptr<int> pSpan = span;

        ReadOnlySpan<int> csp = pArr;
        Span<int> sp = pArr;
        ConstPtr<int> cptr2 = pArr;

        a.Assert(pArr.Length == arr.Length);
        a.Assert(pArr == pSpan);
        a.Assert(cptr1 == cptr2);

        arr[1] = -1;
        pArr[2] = -1;
        pArr.Value = -1;

        a.Assert(pArr[1] == -1);
        a.Assert(arr[2] == -1);
        a.Assert(arr[0] == -1);

        bool sameData = true;
        for (int i = 0; i < pArr.Length; ++i)
            sameData &= arr[i] == pArr[i];

        a.Assert(sameData);

        ReadOnlySpan<int> cspan5 = pSpan[5..];
        pArr = pArr[5..];
        ReadOnlySpan<int> csp5 = pArr;

        a.Assert(csp5 == cspan5);
    }

    [UnitTest]
    public static void Test_Plus(Asserter a)
    {
        int value = Random.Shared.Next(ushort.MaxValue);
        int[] arr = TestData.Generate(100, i => i + value);
        Ptr<int> ptr = arr;

        a.Assert(+ptr == arr[0]);
        a.Assert(+ptr == ptr.Value);

        bool allSameAdd = true;
        for (int i = 0; i < ptr.Length; ++i)
            allSameAdd &= arr[i] == +(ptr + i);

        a.Assert(allSameAdd);
        a.Assert(ptr + 5 - ptr == 5);

        bool isNonEmptyTrueInv = true;
        bool isNonEmptyTrue = true;
        bool allSameInc = true;
        foreach (var i in arr)
        {
            isNonEmptyTrueInv &= !(!ptr);
            if (ptr) { }
            else
                isNonEmptyTrue = false;
            allSameInc &= i == +ptr++;
        }

        a.Assert(allSameInc);
        a.Assert(!ptr);
        a.Assert(isNonEmptyTrue);
        a.Assert(isNonEmptyTrueInv);
    }

    [UnitTest]
    public static void Test_Enumerator(Asserter a)
    {
        int value = 0;// Random.Shared.Next(ushort.MaxValue);
        int[] arr = TestData.Generate(5, i => i + value);
        Ptr<int> ptr = arr;

        bool allSame = true;
        int i = 0;
        foreach (var n in ptr)
            allSame &= arr[i++] == n;

        a.Assert(i == arr.Length);
        a.Assert(allSame);

        foreach (ref var n in ptr)
            n = 5;

        a.Assert(arr[0] == 5);
        a.Assert(Check.AllNeighbours(arr, (a, b) => a == b));
    }
}
