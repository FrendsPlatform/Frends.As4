using System.Collections.Generic;
using System.IO;
using Frends.As4.ValidateAndParsePayload.Definitions;
using NUnit.Framework;

namespace Frends.As4.ValidateAndParsePayload.Tests;

internal abstract class TestBase
{
    protected const string DefaultErrorMessage = "Error occurred";
    private const string CertPassword = "TestCertPass123!";
    private const string ExpectedPayload = "Hello signed/encrypted AS4 World!";
    private const string ExpectedFrom = "SenderParty";
    private const string ExpectedTo = "ReceiverParty";

    private static string TestDataPath =>
        Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");

    private static string PartnerCer => Path.Combine(TestDataPath, "PartnerAS4.cer");

    private static string OwnPfx => Path.Combine(TestDataPath, "OwnCompanyAS4.pfx");

    protected static Input EmptyInput() => new();

    protected static Input InvalidMessage() => new()
    {
        Headers = new Dictionary<string, string> { ["Content-Type"] = "application/soap+xml" },
        Body = "this is not a valid AS4 ebMS message"u8.ToArray(),
    };

    protected static Connection DefaultConnection() => new();

    protected static Options DefaultOptions() => new()
    {
        ThrowErrorOnFailure = false,
        ErrorMessageOnFailure = DefaultErrorMessage,
    };

    protected static Input LoadMessage(string baseName)
    {
        var body = File.ReadAllBytes(Path.Combine(TestDataPath, $"{baseName}.bin"));
        var headers = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadAllLines(Path.Combine(TestDataPath, $"{baseName}.headers.txt")))
        {
            var sep = line.IndexOf(':');
            if (sep > 0)
                headers[line[..sep].Trim()] = line[(sep + 1)..].Trim();
        }

        return new Input
        {
            Body = body,
            Headers = headers,
        };
    }

    protected static Connection SignedConnection() => new()
    {
        RequireSigned = true,
        PartnerCertificatePath = PartnerCer,
        OwnCertificatePath = OwnPfx,
        OwnCertificatePassword = CertPassword,
    };

    protected static Connection EncryptedConnection() => new()
    {
        RequireEncrypted = true,
        OwnCertificatePath = OwnPfx,
        OwnCertificatePassword = CertPassword,
    };

    protected static Connection SignedAndEncryptedConnection() => new()
    {
        RequireSigned = true,
        RequireEncrypted = true,
        PartnerCertificatePath = PartnerCer,
        OwnCertificatePath = OwnPfx,
        OwnCertificatePassword = CertPassword,
    };

    protected static void AssertSuccess(Result result)
    {
        Assert.That(
            result.Success,
            Is.True,
            $"Parsing failed: {result.Error?.Message} {result.Error?.AdditionalInfo}");
        Assert.That(result.As4From, Is.EqualTo(ExpectedFrom));
        Assert.That(result.As4To, Is.EqualTo(ExpectedTo));
        Assert.That(result.Payloads[0], Does.Contain(ExpectedPayload));
        Assert.That(result.Receipt, Is.Not.Null);
    }
}
