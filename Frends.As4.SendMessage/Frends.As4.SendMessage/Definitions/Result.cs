namespace Frends.As4.SendMessage.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The raw receipt content returned by the AS4 partner when using synchronous receipts.
    /// </summary>
    /// <example>&lt;soap:Envelope&gt;...&lt;/soap:Envelope&gt;</example>
    public string PartnerResponse { get; set; }

    /// <summary>
    /// ID of a sent message.
    /// </summary>
    /// <example>123</example>
    public string MessageId { get; set; }

    /// <summary>
    /// Status of receipt handling.
    /// </summary>
    /// <example>ReceiptVerified</example>
    public string ReceiptStatus { get; set; }

    /// <summary>
    /// Human-readable receipt handling message.
    /// </summary>
    /// <example>Synchronous receipt verified.</example>
    public string ReceiptMessage { get; set; }

    /// <summary>
    /// Message Id referenced by the returned receipt, when available.
    /// </summary>
    /// <example>123@nsoftware</example>
    public string ReceiptReferenceMessageId { get; set; }

    /// <summary>
    /// Indicates whether the receipt is expected asynchronously.
    /// </summary>
    /// <example>false</example>
    public bool IsReceiptPending { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}
