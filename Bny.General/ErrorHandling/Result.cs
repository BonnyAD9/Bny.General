namespace Bny.General.ErrorHandling;

/// <summary>
/// Represents a result with no data that can be successful or unsuccessful
/// </summary>
/// Exception that should be thrown on failure
public class Result
{
    /// <summary>
    /// Value indicating whether the operation was successful or not
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// True if the operation failed, otherwise false
    /// </summary>
    public bool Failed => !Success;

    /// <summary>
    /// Message, usually describes the failure
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Type of exception to throw on failure
    /// </summary>
    protected Type? ExceptionType { get; init; }

    /// <summary>
    /// Creates new result with the given excpetion type
    /// </summary>
    /// <param name="success">
    /// Value indicating success of the operation
    /// </param>
    /// <param name="message">message in case of failure</param>
    /// <param name="exceptionType">
    /// Type of exception to throw in case of failure must be an exception
    /// </param>
    protected Result(bool success, string? message, Type? exceptionType)
    {
        if (!success && exceptionType is null)
            exceptionType = typeof(Exception);
        if (!typeof(Exception).IsAssignableFrom(exceptionType))
            throw new ArgumentException(
                "type must be exception", nameof(exceptionType));

        Success = success;
        Message = message;
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// Creates new result
    /// </summary>
    /// <param name="success">
    /// Value indicating whether the operation was successful
    /// </param>
    /// <param name="message">Message describing failure</param>
    public Result(bool success, string? message)
    {
        Success = success;
        Message = message;
        if (Failed)
            ExceptionType = typeof(Exception);
    }

    /// <summary>
    /// Creates new result with the success value and no message
    /// </summary>
    /// <param name="success">
    /// Value indicating whether the operation was successful
    /// </param>
    public Result(bool success) : this(success, null) { }

    /// <summary>
    /// Creates unsuccessful result with the given message
    /// </summary>
    /// <param name="message">Description of the failure</param>
    public Result(string? message) : this(false, message) { }

    /// <summary>
    /// Creates successful result with no message
    /// </summary>
    public Result() : this(true) { }

    /// <summary>
    /// Throws the exception if Success is false
    /// </summary>
    public void ThrowOnFail()
    {
        if (Success)
            return;

        if (Message is null)
            throw (Exception)Activator.CreateInstance(ExceptionType!)!;
        throw (Exception)Activator.CreateInstance(
            ExceptionType!, Message ?? "")!;
    }

    /// <inheritdoc/>
    public static bool operator true(Result r) => r.Success;

    /// <inheritdoc/>
    public static bool operator false(Result r) => r.Failed;

    /// <inheritdoc/>
    public static bool operator !(Result r) => r.Failed;
}
