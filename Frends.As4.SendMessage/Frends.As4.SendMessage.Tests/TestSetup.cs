using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
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

    public static Connection HttpsConnection()
    {
        var connection = Connection();
        connection.As4EndpointUrl = "https://localhost:4443";

        return connection;
    }

    public static async Task<string> GetServerCertificateBase64Async(CancellationToken token)
    {
        using var client = new TcpClient();
        await client.ConnectAsync("localhost", 4443, token);

        await using var ssl = new SslStream(client.GetStream(), false, (_, _, _, _) => true);
        await ssl.AuthenticateAsClientAsync(
            new SslClientAuthenticationOptions { TargetHost = "localhost" },
            token);

        using var certificate = new X509Certificate2(ssl.RemoteCertificate);
        return Convert.ToBase64String(certificate.Export(X509ContentType.Cert));
    }

    public static string GetSenderCertificateBase64()
    {
        var pem = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "certs", "sender.pem"));
        var der = Convert.FromBase64String(
            pem.Replace("-----BEGIN CERTIFICATE-----", string.Empty)
                .Replace("-----END CERTIFICATE-----", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty));
        using var certificate = new X509Certificate2(der);

        return Convert.ToBase64String(certificate.Export(X509ContentType.Cert));
    }

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
