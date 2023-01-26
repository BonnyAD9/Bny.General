namespace Bny.General.ErrorHandling;

/// <summary>
/// Represents result with value and exception that can be successful or not
/// </summary>
/// <typeparam name="T">Type of the value</typeparam>
/// <typeparam name="Ex">Type of exception to throw on failure</typeparam>
public class Result<T, Ex> : Result<T> where Ex : Exception
{
    /// <summary>
    /// Creates new result with the success value and message
    /// </summary>
    /// <param name="value">Result value</param>
    /// <param name="success">Success value</param>
    /// <param name="message">Message in case of failure</param>
    public Result(T value, bool success, string? message)
        : base(value, success, message)
    {
        ExceptionType = typeof(T);
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
    /// Creates new successful result from the value
    /// </summary>
    /// <param name="value">Return value of the result</param>
    public static implicit operator Result<T, Ex>(T value) => new(value);

    /// <summary>
    /// Gets the value if success is true, otherwise throws
    /// </summary>
    /// <param name="res">Result to get the value from</param>
    public static explicit operator T(Result<T, Ex> res) => res.GetOrThrow();
}
