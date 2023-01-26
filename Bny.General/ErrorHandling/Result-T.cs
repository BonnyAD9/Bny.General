namespace Bny.General.ErrorHandling;

/// <summary>
/// Represents result with value that can be successful or not
/// </summary>
/// <typeparam name="T">Type of the value</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Return value, result of the operation
    /// </summary>
    public T Value { get; init; }

    /// <summary>
    /// Creates new result with all the parameters
    /// </summary>
    /// <param name="value">Result value</param>
    /// <param name="success">Determines the success of the operation</param>
    /// <param name="message">Message in case of failure</param>
    /// <param name="exceptionType">
    /// Type of exception to throw in case of failure
    /// </param>
    protected Result(
        T value, bool success, string? message, Type? exceptionType)
        : base(success, message, exceptionType)
    {
        Value = value;
    }

    /// <summary>
    /// Creates new result with the success value and message
    /// </summary>
    /// <param name="value">Result value</param>
    /// <param name="success">Success value</param>
    /// <param name="message">Message in case of failure</param>
    public Result(T value, bool success, string? message)
        : base(success, message)
    {
        Value = value;
    }

    /// <summary>
    /// Creates new result with the success value
    /// </summary>
    /// <param name="value">Result value</param>
    /// <param name="success">Success value</param>
    public Result(T value, bool success) : this(value, success, null) { }

    /// <summary>
    /// Creates new unsuccessful result with value
    /// </summary>
    /// <param name="value">Result value</param>
    /// <param name="message">Failure description</param>
    public Result(T value, string? message) : this(value, false, message) { }

    /// <summary>
    /// Creates new unsuccessful result with no value
    /// </summary>
    /// <param name="message">Failure description</param>
    public Result(string? message) : this(default!, message) { }

    /// <summary>
    /// Creates new successful result with value
    /// </summary>
    /// <param name="value">Result value</param>
    public Result(T value) : this(value, true) { }

    /// <summary>
    /// Throws on fail, returns on success
    /// </summary>
    /// <returns>Result value</returns>
    public T GetOrThrow()
    {
        ThrowOnFail();
        return Value;
    }

    /// <summary>
    /// Compares the two success and result values
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns>
    /// True if both results have the same success and result value
    /// </returns>
    public static bool operator ==(Result<T> l, Result<T> r)
        => l.Success == r.Success && l.Value!.Equals(r.Value);

    /// <summary>
    /// Compares the two success and result values
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns>
    /// False if both results have the same success and result value
    /// </returns>
    public static bool operator !=(Result<T> l, Result<T> r)
        => l.Success != r.Success || !l.Value!.Equals(r.Value);

    /// <summary>
    /// Compares the result value if success
    /// </summary>
    /// <param name="l">Result to compare</param>
    /// <param name="r">Value to compare</param>
    /// <returns>
    /// True if the result is success and the result value is same as
    /// <paramref name="r"/>
    /// </returns>
    public static bool operator ==(Result<T> l, T r)
        => l.Success && l.Value!.Equals(r);

    /// <summary>
    /// Compares the result value if success
    /// THIS IS NOT NEGATION OF THE == OPERATOR!!
    /// </summary>
    /// <param name="l">Result to compare</param>
    /// <param name="r">Value to compare</param>
    /// <returns>
    /// True if the result is success and the result value is not same as
    /// <paramref name="r"/>
    /// </returns>
    [Obsolete("'l != r' is not the same as '!(l == r)'")]
    public static bool operator !=(Result<T> l, T r)
        => l.Success && !l.Value!.Equals(r);

    /// <summary>
    /// Creates new successful result from the value
    /// </summary>
    /// <param name="value">Return value of the result</param>
    public static implicit operator Result<T>(T value) => new(value);

    /// <summary>
    /// Gets the value if success is true, otherwise throws
    /// </summary>
    /// <param name="res">Result to get the value from</param>
    public static explicit operator T(Result<T> res) => res.GetOrThrow();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => base.Equals(obj);

    /// <inheritdoc/>
    public override int GetHashCode() => base.GetHashCode();
}
