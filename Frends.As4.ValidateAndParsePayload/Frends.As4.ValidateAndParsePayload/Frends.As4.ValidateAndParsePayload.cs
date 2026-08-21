using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Frends.As4.ValidateAndParsePayload.Definitions;
using Frends.As4.ValidateAndParsePayload.Helpers;
using nsoftware.async.IPWorksEDI;

namespace Frends.As4.ValidateAndParsePayload;

/// <summary>
/// Task Class for As4 operations.
/// </summary>
public static class As4
{
    /// <summary>
    /// Task to validate an incoming AS4 message, extract the EDI payload, and generate a receipt.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-As4-ValidateAndParsePayload)
    /// </summary>
    /// <param name="input">Essential parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object {
    /// bool Success, string Payload, string As4From, string As4To, string MessageId, ReceiptData Receipt, object Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> ValidateAndParsePayload(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            ValidationHandler.Run(input, connection);

            var as4 = NSoftware.Activation.NSoftware.ActivateAs4Server();
            as4.RequestHeadersString = ConvertHeadersToString(input.Headers);

            using var ms = new MemoryStream(input.Body);
            await as4.SetRequestStream(ms, cancellationToken);

            if (connection.RequireSigned)
                as4.SignerCert = new Certificate(connection.PartnerCertificatePath);

            if (connection.RequireSigned || connection.RequireEncrypted)
            {
                as4.Certificate = new Certificate(
                    CertStoreTypes.cstPFXFile,
                    connection.OwnCertificatePath,
                    connection.OwnCertificatePassword,
                    "*");
            }

            await as4.ParseRequest(cancellationToken);
            var payloads = as4.EDIData.Select(x => x.Data).ToArray();

            return new Result
            {
                Success = true,
                As4From = as4.AS4From?.Id,
                As4To = as4.AS4To?.Id,
                MessageId = as4.MessageId,
                Payloads = payloads,
                Receipt = new ReceiptData
                {
                    Content = as4.Receipt?.Content,
                    RefToMessageId = as4.Receipt?.RefToMessageId,
                },
            };
        }
        catch (Exception e)
        {
            return e.Handle(options);
        }
    }

    internal static string ConvertHeadersToString(Dictionary<string, string> headers)
    {
        var sb = new StringBuilder();
        foreach (var header in headers)
            sb.Append($"{header.Key}: {header.Value}\r\n");

        return sb.ToString();
    }
}
