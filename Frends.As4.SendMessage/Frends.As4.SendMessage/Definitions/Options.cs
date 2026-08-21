using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.As4.SendMessage.Attributes;

namespace Frends.As4.SendMessage.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Specifies whether to request a synchronous or asynchronous AS4 receipt.
    /// </summary>
    /// <example>Sync</example>
    [DefaultValue(ReceiptMode.Sync)]
    public ReceiptMode ReceiptMode { get; set; } = ReceiptMode.Sync;

    /// <summary>
    /// Directory where the component stores correlation data for async receipt verification.
    /// Required when asynchronous receipts are enabled.
    /// </summary>
    /// <example>C:\Temp\Frends\As4\AsyncReceipts</example>
    [RequiredIf(nameof(ReceiptMode), ReceiptMode.Async)]
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(ReceiptMode), "", ReceiptMode.Async)]
    public string AsyncReceiptInfoDirectory { get; set; }

    /// <summary>
    /// Directory where transport logs are written.
    /// If left empty, logging is not enabled.
    /// </summary>
    /// <example>C:\Temp\Frends\As4\Logs</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string LogDirectory { get; set; }

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}
