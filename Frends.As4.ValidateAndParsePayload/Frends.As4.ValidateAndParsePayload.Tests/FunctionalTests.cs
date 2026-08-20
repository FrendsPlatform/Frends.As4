using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Frends.As4.ValidateAndParsePayload.Tests;

[TestFixture]
internal class FunctionalTests : TestBase
{
    [Test]
    public async Task Should_Return_Failure_When_Input_Empty()
    {
        var result = await As4.ValidateAndParsePayload(
            EmptyInput(),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Validation failed"));
    }

    [Test]
    public void Should_Throw_When_Input_Empty_And_Flag_Enabled()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = true;

        Assert.ThrowsAsync<System.Exception>((System.Func<System.Threading.Tasks.Task>)(async () =>
            await As4.ValidateAndParsePayload(
                EmptyInput(),
                DefaultConnection(),
                options,
                CancellationToken.None)));
    }

    [Test]
    public async Task Should_Return_Failure_With_Custom_Error_Message()
    {
        var result = await As4.ValidateAndParsePayload(
            EmptyInput(),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain(DefaultErrorMessage));
    }

    [Test]
    public async Task Should_Return_Failure_On_Invalid_Message()
    {
        var result = await As4.ValidateAndParsePayload(
            InvalidMessage(),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.AdditionalInfo, Is.Not.Null);
    }

    [Test]
    public void ConvertHeadersToString_Should_Format_Headers()
    {
        var headers = new Dictionary<string, string>
        {
            ["Content-Type"] = "application/soap+xml",
            ["SOAPAction"] = "\"\"",
        };

        var result = As4.ConvertHeadersToString(headers);

        Assert.That(result, Is.EqualTo("Content-Type: application/soap+xml\r\nSOAPAction: \"\"\r\n"));
    }

    /// <summary>
    /// Reads a real AS4/ebMS3 MIME message from disk and verifies that all fields
    /// are parsed correctly.
    ///
    /// Expected message values (see TestData/sample_as4_message.mime):
    ///   As4From    = "SenderParty"
    ///   As4To      = "ReceiverParty"
    ///   MessageId  = "testmessage-001@frends.com"
    ///   Payload    = "Hello AS4 World! This is the test payload for Frends AS4 task."
    /// </summary>
    [Test]
    public async Task Should_Parse_Valid_As4_Message_From_File()
    {
        // ---- arrange -------------------------------------------------------
        var testFilePath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "TestData",
            "sample_as4_message.mime");

        Assert.That(File.Exists(testFilePath), Is.True,
            $"Test data file not found: {testFilePath}");

        var body = await File.ReadAllBytesAsync(testFilePath);

        // The Content-Type header must reference the same MIME boundary that is
        // used inside the file and must point to the root SOAP part.
        var headers = new Dictionary<string, string>
        {
            ["Content-Type"] =
                "multipart/related; " +
                "type=\"application/soap+xml\"; " +
                "boundary=\"MIMEBoundary_frends_as4_test\"; " +
                "start=\"<rootpart@frends.com>\"",
        };

        var input = new Frends.As4.ValidateAndParsePayload.Definitions.Input
        {
            Headers = headers,
            Body = body,
        };

        // ---- act -----------------------------------------------------------
        var result = await As4.ValidateAndParsePayload(
            input,
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        // ---- assert --------------------------------------------------------
        Assert.That(result.Success, Is.True,
            $"Parsing failed: {result.Error?.Message}\n{result.Error?.AdditionalInfo}");

        Assert.That(result.As4From, Is.EqualTo("SenderParty"),
            "AS4 sender party (From) did not match expected value.");

        Assert.That(result.As4To, Is.EqualTo("ReceiverParty"),
            "AS4 receiver party (To) did not match expected value.");

        Assert.That(result.MessageId, Is.EqualTo("testmessage-001@frends.com"),
            "MessageId did not match expected value.");

        Assert.That(result.Payload, Does.Contain("Hello AS4 World!"),
            "Payload content did not match expected value.");

        Assert.That(result.Receipt, Is.Not.Null,
            "Receipt should be generated for a valid UserMessage.");
    }
}
