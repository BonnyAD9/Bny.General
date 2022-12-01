namespace Bny.General.Memory;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

/// <summary>
/// Wrapper for span, so that it can be casted to object
/// Should be used only on fixed memory
/// </summary>
/// <typeparam name="T">Type of data in the Span</typeparam>
public unsafe readonly struct ReadOnlySpanWrapper<T>
{
    private readonly T* _ptr;
    private readonly int _size;

    /// <summary>
    /// Length of the span
    /// </summary>
    public int Length => _size;

    /// <summary>
    /// Creates SpanWrapper from a read only span
    /// This should be used only when the span memory is fixed
    /// </summary>
    /// <param name="span">Span to wrap, its memory should be fixed</param>
    public ReadOnlySpanWrapper(ReadOnlySpan<T> span)
    {
        fixed (T* ptr = span)
            _ptr = ptr;
        _size = span.Length;
    }

    /// <summary>
    /// Creates SpanWrapper from a span
    /// This should be used only when the span memory is fixed
    /// </summary>
    /// <param name="span">Span to wrap, its memory should be fixed</param>
    public ReadOnlySpanWrapper(Span<T> span)
    {
        fixed (T* ptr = span)
            _ptr = ptr;
        _size = span.Length;
    }

    /// <summary>
    /// Gets the span
    /// </summary>
    /// <returns>The wrapped span</returns>
    public ReadOnlySpan<T> GetSpan() => new(_ptr, _size);

    /// <summary>
    /// Unwraps the span
    /// </summary>
    /// <param name="wrappedSpan">Span that should be unwapped</param>
    public static implicit operator ReadOnlySpan<T>(ReadOnlySpanWrapper<T> wrappedSpan) => wrappedSpan.GetSpan();
}

#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type