using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Frends.As4.ValidateAndParsePayload.Attributes;

namespace Frends.As4.ValidateAndParsePayload.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// HTTP headers from the AS4 request containing SOAPAction, Content-Type and other AS4/ebMS metadata.
    /// </summary>
    /// <example>
    /// {
    ///     "Content-Type": "multipart/related; boundary=\"MIME_boundary\"; type=\"application/soap+xml\"",
    ///     "SOAPAction": "\"\"",
    ///     "Content-Length": "12345"
    /// }
    /// </example>
    [NotEmpty]
    public Dictionary<string, string> Headers { get; set; } = [];

    /// <summary>
    /// Raw body content as byte array containing the AS4 (ebMS) message payload (signed/encrypted SOAP with attachments).
    /// </summary>
    /// <example>
    /// Encoding.UTF8.GetBytes("--MIME_boundary...")
    /// </example>
    [NotEmpty]
    public byte[] Body { get; set; } = [];
}
