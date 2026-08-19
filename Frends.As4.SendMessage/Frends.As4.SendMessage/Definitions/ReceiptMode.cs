namespace Frends.As4.SendMessage.Definitions;

/// <summary>
/// Specifies how the AS4 receipt should be handled.
/// </summary>
public enum ReceiptMode
{
    /// <summary>
    /// Synchronous receipt mode. The sender waits for the receipt immediately after sending the message.
    /// </summary>
    Sync,

    /// <summary>
    /// Asynchronous receipt mode. The sender requests the receipt to be returned later,
    /// allowing the send operation to complete without waiting for it.
    /// </summary>
    Async,
}
