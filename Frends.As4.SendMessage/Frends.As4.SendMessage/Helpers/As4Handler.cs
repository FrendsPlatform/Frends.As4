using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Frends.As4.SendMessage.Definitions;
using nsoftware.async.IPWorksEDI;

namespace Frends.As4.SendMessage.Helpers;

internal static class As4Handler
{
    internal static void ConfigureMessage(AS4Client as4, Input input, Connection connection, Options options)
    {
        as4.AS4From = new EBPartyInfo
        {
            Id = input.SenderAs4Id,
            Role = "Sender",
        };
        as4.AS4To = new EBPartyInfo
        {
            Id = input.ReceiverAs4Id,
            Role = "Receiver",
        };
        as4.AgreementRef = string.IsNullOrWhiteSpace(connection.AgreementRef)
            ? $"{input.SenderAs4Id}:{input.ReceiverAs4Id}"
            : connection.AgreementRef;
        as4.URL = connection.As4EndpointUrl;

        var uri = new Uri(connection.As4EndpointUrl);
        as4.MessageId = $"{Guid.NewGuid()}@{uri.Host}";
        as4.ReceiptReplyMode = options.ReceiptMode == ReceiptMode.Async
            ? AS4ClientReceiptReplyModes.rrmAsync
            : AS4ClientReceiptReplyModes.rrmSync;

        if (options.ReceiptMode == ReceiptMode.Async)
        {
            Directory.CreateDirectory(options.AsyncReceiptInfoDirectory);
            as4.AsyncReceiptInfoDir = options.AsyncReceiptInfoDirectory;
        }

        if (!string.IsNullOrWhiteSpace(input.Subject))
        {
            as4.MessageProperties.Add(new EBProperty("Subject", input.Subject));
        }

        if (input.AdditionalHeaders == null)
        {
            return;
        }

        foreach (var property in input.AdditionalHeaders)
        {
            if (string.IsNullOrWhiteSpace(property?.Name))
            {
                continue;
            }

            if (property.Name.Equals("AS4-From", StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals("AS4-To", StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals("Message-ID", StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals("Subject", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            as4.MessageProperties.Add(new EBProperty(property.Name, property.Value));
        }
    }

    internal static void ConfigureSecurity(AS4Client as4, Connection connection)
    {
        var receiverCertificate = new Certificate(connection.ReceiverCertificatePath);
        as4.SignerCert = receiverCertificate;

        if (connection.EncryptMessage)
        {
            as4.RecipientCerts.Add(receiverCertificate);
        }
        else
        {
            as4.EncryptionAlgorithm = string.Empty;
        }

        if (connection.SignMessage)
        {
            as4.SigningCert = new Certificate(
                CertStoreTypes.cstAuto,
                connection.SenderCertificatePath,
                connection.SenderCertificatePassword,
                "*");
        }
    }

    internal static async Task ConfigurePayload(
        AS4Client as4,
        Input input,
        Connection connection,
        CancellationToken cancellationToken)
    {
        var data = new EBData
        {
            EDIType = connection.ContentTypeHeader,
            Name = Path.GetFileName(input.MessageFilePath),
        };

        data.Data = await File.ReadAllTextAsync(input.MessageFilePath!, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        as4.EDIData.Add(data);
    }

    internal static void ConfigureLogging(AS4Client as4, Options options)
    {
        if (string.IsNullOrWhiteSpace(options.LogDirectory)) return;
        Directory.CreateDirectory(options.LogDirectory);
        as4.LogDirectory = options.LogDirectory;
    }

    internal static Result CreateResult(AS4Client as4, Options options)
    {
        var isAsyncReceipt = options.ReceiptMode == ReceiptMode.Async;
        var receipt = as4.Receipt;

        return new Result
        {
            Success = true,
            PartnerResponse = isAsyncReceipt ? null : receipt?.Content,
            MessageId = as4.MessageId,
            IsReceiptPending = isAsyncReceipt,
            ReceiptStatus = isAsyncReceipt ? "AsyncReceiptPending" : "ReceiptVerified",
            ReceiptMessage = isAsyncReceipt
                ? $"Async receipt requested. Correlation data stored in: {as4.AsyncReceiptInfoDir}"
                : "Synchronous receipt verified.",
            ReceiptReferenceMessageId = receipt?.RefToMessageId,
        };
    }
}
