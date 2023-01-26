namespace Bny.General.ErrorHandling;

/// <summary>
/// Represents a result with no data that can be successful or unsuccessful
/// </summary>
/// <typeparam name="Ex">
/// Exception that should be thrown on failure
/// </typeparam>
public class ResultEx<Ex> : Result where Ex : Exception
{

    /// <summary>
    /// Creates new result
    /// </summary>
    /// <param name="success">
    /// Value indicating whether the operation was successful
    /// </param>
    /// <param name="message">Message describing failure</param>
    public ResultEx(bool success, string? message) : base(success, message)
    {
        ExceptionType = typeof(Ex);
    }

    /// <summary>
    /// Creates new result with the success value and no message
    /// </summary>
    /// <param name="success">
    /// Value indicating whether the operation was successful
    /// </param>
    public ResultEx(bool success) : this(success, null) { }

    /// <summary>
    /// Creates unsuccessful result with the given message
    /// </summary>
    /// <param name="message">Description of the failure</param>
    public ResultEx(string? message) : this(false, message) { }

    /// <summary>
    /// Creates successful result with no message
    /// </summary>
    public ResultEx() : this(true) { }
}
