using System;
using System.IO;
using Frends.As4.SendMessage.Definitions;

namespace Frends.As4.SendMessage.Tests;

public static class TestSetup
{
    private static readonly string TestFilePath = Path.Combine(AppContext.BaseDirectory, "testData", "mess.txt");

    public static Input Input() => new()
    {
        SenderAs4Id = "Sender",
        ReceiverAs4Id = "Receiver",
        Subject = "Test Connection",
        MessageFilePath = TestFilePath,
    };

    public static Connection Connection() =>
        new()
        {
            As4EndpointUrl = "http://localhost:4080",
            SignMessage = false,
            EncryptMessage = false,
            SenderCertificatePassword = "sender123",
            SenderCertificatePath = Path.Combine(AppContext.BaseDirectory, "certs", "sender.pfx"),
            ReceiverCertificatePath = Path.Combine(AppContext.BaseDirectory, "certs", "receiver.pem"),
            AgreementRef = "TestAgreementRef",
            ContentTypeHeader = "text/plain",
        };

    public static Options Options() => new()
    {
        ThrowErrorOnFailure = false,
        ErrorMessageOnFailure = null,
    };

    public static Options AsyncOptions() => new()
    {
        ReceiptMode = ReceiptMode.Async,
        ThrowErrorOnFailure = false,
        ErrorMessageOnFailure = null,
        AsyncReceiptInfoDirectory = Path.Combine(
            Path.GetTempPath(),
            "Frends.As4.SendMessage.Tests",
            "async-receipts",
            Guid.NewGuid().ToString("N")),
    };
}
