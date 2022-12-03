using Bny.General.Memory;

namespace Bny.General.Tester;

[UnitTest]
internal class ReadOnlySpanWrapperTester
{
    [UnitTest]
    public static unsafe void Test_Conversion(Asserter a)
    {
        int[] arr = TestData.Generate(100, i => i);
        ReadOnlySpan<int> originalReadOnlySpan = arr;
        Span<int> originalSpan = arr;

        fixed (int* ptr = originalSpan)
        {
            ReadOnlySpanWrapper<int> readOnlyWrapper =
                new(originalReadOnlySpan);
            ReadOnlySpanWrapper<int> wrapper = new(originalSpan);

            object readOnlyWrapperAsObject = readOnlyWrapper;
            object wrapperAsObject = wrapper;

            readOnlyWrapper =
                (ReadOnlySpanWrapper<int>)readOnlyWrapperAsObject;
            wrapper = (ReadOnlySpanWrapper<int>)wrapperAsObject;

            ReadOnlySpan<int> unwrappedReadOnlySpan = readOnlyWrapper;
            ReadOnlySpan<int> unwrappedSpan = wrapper;

            a.Assert(readOnlyWrapper.Length == originalReadOnlySpan.Length);
            a.Assert(wrapper.Length == originalSpan.Length);
            a.Assert(originalReadOnlySpan == unwrappedReadOnlySpan);
            a.Assert(originalSpan == unwrappedSpan);
        }
    }
}
