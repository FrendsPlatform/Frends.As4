using System;

namespace Frends.As4.SendMessage.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to send As4 message.</example>
    public string Message { get; init; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception Exception }</example>
    public Exception AdditionalInfo { get; set; }
}
