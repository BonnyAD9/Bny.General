using Bny.General.Memory;

namespace Bny.General.Tester.Memory;

[UnitTest]
internal class ConstPtrExtensionsTester
{
    [UnitTest]
    public static void Test_IndexOf1(Asserter a)
    {
        int value = Random.Shared.Next(ushort.MaxValue);
        ConstPtr<int> ptr = TestData.Generate(100, i => i + value);
        int index = Random.Shared.Next(100);

        a.Assert(ptr.IndexOf(index + value) == index);
        a.Assert(ptr.IndexOf(index + 100 + value) == -1);
    }

    [UnitTest]
    public static void Test_StartsWith(Asserter a)
    {
        int value = Random.Shared.Next(ushort.MaxValue);
        ConstPtr<int> ptr = TestData.Generate(100, i => i + value);

        a.Assert(ptr.StartsWith(ptr[..5]));
        a.Assert(!ptr[..5].StartsWith(ptr));
        a.Assert(!ptr.StartsWith(ptr[1..]));
    }

    [UnitTest]
    public static void Test_IndexOfMultiple(Asserter a)
    {
        int value = Random.Shared.Next(ushort.MaxValue);
        ConstPtr<int> ptr = TestData.Generate(100, i => i + value);
        int index = Random.Shared.Next(90);
        ConstPtr<int> other = TestData.Generate(10, i => i + index + value);

        a.Assert(ptr.IndexOf(other) == index);
        a.Assert(ptr.IndexOf(new int[] { 3, 2, 1 }) == -1);
    }

    [UnitTest]
    public static void Test_IndexOfRepeat(Asserter a)
    {
        int value = Random.Shared.Next(ushort.MaxValue);
        ConstPtr<int> ptr = TestData.Generate(100, i => i + value);
        ConstPtr<int> other1 = new int[] { value + 6, value + 7 };
        ConstPtr<int> other2 = new int[] { value + 7, value + 8 };

        a.Assert(ptr.IndexOfRepeat(other1) == 6);
        a.Assert(ptr.IndexOfRepeat(other2) == -1);
    }

    [UnitTest]
    public static void Test_Equals(Asserter a)
    {
        ConstPtr<char> p1 = "hello".AsSpan();

        a.Assert(p1.HasSameContents(p1));
        a.Assert(p1.HasSameContents("hello".AsSpan()));
        a.Assert(!p1.HasSameContents(p1[1..]));
        a.Assert(!p1[1..].HasSameContents(p1[..^1]));
    }

    [UnitTest]
    public static void Test_ToArray(Asserter a)
    {
        ConstPtr<char> p1 = "hello".AsSpan();

        a.Assert(p1.HasSameContents(p1.ToArray()));
    }
}
