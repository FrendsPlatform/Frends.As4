using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.As4.SendMessage.Attributes;

namespace Frends.As4.SendMessage.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Connection string to AS4 server.
    /// </summary>
    /// <example>https://as4.example.com/msh</example>
    [Required]
    [DisplayFormat(DataFormatString = "Text")]
    public string As4EndpointUrl { get; set; }

    /// <summary>
    /// Defines whether to sign the message.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    public bool SignMessage { get; set; }

    /// <summary>
    /// Defines whether to encrypt the message.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    public bool EncryptMessage { get; set; }

    /// <summary>
    /// Password for the sender certificate.
    /// </summary>
    /// <example>mySecurePassword123</example>
    [RequiredIf(nameof(SignMessage), true)]
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [UIHint(nameof(SignMessage), "", true)]
    public string SenderCertificatePassword { get; set; }

    /// <summary>
    /// Path to the sender certificate file in .pfx format.
    /// </summary>
    /// <example>C:\Document\sender_cert.pfx</example>
    [RequiredIf(nameof(SignMessage), true)]
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(SignMessage), "", true)]
    public string SenderCertificatePath { get; set; }

    /// <summary>
    /// Path to the receiver public certificate file used for encryption and receipt signature verification.
    /// </summary>
    /// <example>C:\Document\receiver_cert.pem</example>
    [Required]
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(EncryptMessage), "", true)]
    public string ReceiverCertificatePath { get; set; }

    /// <summary>
    /// Agreement identifier shared by both AS4 parties. If not provided the agreement will be generated automatically in form "SenderId:ReceiverId".
    /// </summary>
    /// <example>urn:agreement:my-company:partner</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string AgreementRef { get; set; }

    /// <summary>
    /// Payload content type for the file being sent.
    /// </summary>
    /// <example>application/zip</example>
    [Required]
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("text/plain")]
    public string ContentTypeHeader { get; set; } = "text/plain";
}
