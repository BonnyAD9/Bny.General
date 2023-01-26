using Bny.General.Memory;

namespace Bny.General.Tester.Memory;

[UnitTest]
internal class SpanWrapperTester
{
    [UnitTest]
    public static unsafe void Test_Conversion(Asserter a)
    {
        int[] arr = TestData.Generate(100, i => i);
        Span<int> originalSpan = arr;

        fixed (int* ptr = originalSpan)
        {
            SpanWrapper<int> wrapper = new(originalSpan);

            object wrapperAsObject = wrapper;

            wrapper = (SpanWrapper<int>)wrapperAsObject;

            Span<int> unwrappedSpan = wrapper;
            ReadOnlySpan<int> unwrappedReadOnlySpan = wrapper;

            a.Assert(wrapper.Length == originalSpan.Length);
            a.Assert(originalSpan == unwrappedSpan);
            a.Assert(originalSpan == unwrappedReadOnlySpan);
        }
    }
}
