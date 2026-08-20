namespace Frends.As4.ValidateAndParsePayload.Definitions;

/// <summary>
/// Contains AS4 (ebMS) receipt data generated after processing an AS4 message.
/// The receipt serves as a non-repudiation confirmation that the AS4 message was received and processed.
/// </summary>
public class ReceiptData
{
    /// <summary>
    /// Raw receipt content (signed ebMS receipt SOAP message) returned by the AS4Server.
    /// </summary>
    /// <example>"&lt;eb:SignalMessage&gt;...&lt;/eb:SignalMessage&gt;"</example>
    public string Content { get; set; }

    /// <summary>
    /// The MessageId of the original message this receipt refers to.
    /// </summary>
    /// <example>"123456789@example.com"</example>
    public string RefToMessageId { get; set; }
}
