namespace Bny.General.Memory;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

/// <summary>
/// Wrapper for span, so that it can be casted to object
/// </summary>
/// <typeparam name="T">Type of data in the Span</typeparam>
public unsafe readonly struct SpanWrapper<T>
{
    private readonly T* _ptr;
    private readonly int _size;

    /// <summary>
    /// Length of the span
    /// </summary>
    public int Length => _size;

    /// <summary>
    /// Creates SpanWrapper from a span
    /// This should be used only when the span memory is fixed
    /// </summary>
    /// <param name="span">Span to wrap, its memory should be fixed</param>
    public SpanWrapper(Span<T> span)
    {
        fixed (T* ptr = span)
            _ptr = ptr;
        _size = span.Length;
    }

    /// <summary>
    /// Gets the span
    /// </summary>
    /// <returns>The wrapped span</returns>
    public Span<T> GetSpan() => new(_ptr, _size);

    /// <summary>
    /// Gets the read only version of the wapped span
    /// </summary>
    /// <returns>Read only version on wrapped span</returns>
    public Span<T> GetReadOnlySpan() => new(_ptr, _size);

    /// <summary>
    /// Unwraps the span
    /// </summary>
    /// <param name="wrappedSpan">Span that should be unwapped</param>
    public static implicit operator Span<T>(SpanWrapper<T> wrappedSpan) => wrappedSpan.GetSpan();

    /// <summary>
    /// Unwraps the span as read only
    /// </summary>
    /// <param name="wrappedSpan">Span to be unwrapped</param>
    public static implicit operator ReadOnlySpan<T>(SpanWrapper<T> wrappedSpan) => wrappedSpan.GetReadOnlySpan();
}

#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type