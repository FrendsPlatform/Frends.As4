using System.ComponentModel.DataAnnotations;

namespace Frends.As4.SendMessage.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Id of the company that will send the message.
    /// </summary>
    /// <example>MyCompany</example>
    [Required]
    [DisplayFormat(DataFormatString = "Text")]
    public string SenderAs4Id { get; set; }

    /// <summary>
    /// Id of the company that will receive the message.
    /// </summary>
    /// <example>YourCompany</example>
    [Required]
    [DisplayFormat(DataFormatString = "Text")]
    public string ReceiverAs4Id { get; set; }

    /// <summary>
    /// Optional subject that will be added as an AS4 message property.
    /// </summary>
    /// <example>Subject of the message</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Subject { get; set; }

    /// <summary>
    /// Path to the file that will be sent in the message.
    /// </summary>
    /// <example>C:\Document\message.txt</example>
    [Required]
    [DisplayFormat(DataFormatString = "Text")]
    public string MessageFilePath { get; set; }

    /// <summary>
    /// Optional additional AS4 message properties to send with the message.
    /// </summary>
    /// <example>[{ "Name": "X-ApiKey", "Value": "my-api-key" }]</example>
    public Header[] AdditionalHeaders { get; set; }
}
