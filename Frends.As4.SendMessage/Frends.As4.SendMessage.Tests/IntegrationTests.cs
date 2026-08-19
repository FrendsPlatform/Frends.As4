using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Frends.As4.SendMessage.Tests;

[TestFixture]
public class IntegrationTests
{
    [Test]
    public async Task ShouldSendPlainMessage()
    {
        var result = await As4.SendMessage(
            TestSetup.Input(),
            TestSetup.Connection(),
            TestSetup.Options(),
            CancellationToken.None);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task ShouldSendSignedMessage()
    {
        var con = TestSetup.Connection();
        con.SignMessage = true;

        var result = await As4.SendMessage(TestSetup.Input(), con, TestSetup.Options(), CancellationToken.None);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task ShouldSendEncryptedMessage()
    {
        var con = TestSetup.Connection();
        con.EncryptMessage = true;

        var result = await As4.SendMessage(TestSetup.Input(), con, TestSetup.Options(), CancellationToken.None);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task ShouldSendSignedAndEncryptedMessage()
    {
        var con = TestSetup.Connection();
        con.SignMessage = true;
        con.EncryptMessage = true;

        var opt = TestSetup.Options();
        opt.ThrowErrorOnFailure = true;

        var result = await As4.SendMessage(TestSetup.Input(), con, opt, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(result.IsReceiptPending, Is.False);
    }

    [Test]
    public async Task ShouldSendMessageWithAsyncReceipt()
    {
        var opt = TestSetup.AsyncOptions();

        var result = await As4.SendMessage(TestSetup.Input(), TestSetup.Connection(), opt, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.IsReceiptPending, Is.True);
        Assert.That(result.ReceiptStatus, Is.EqualTo("AsyncReceiptPending"));
    }

    [Test]
    public async Task ShouldSendSignedMessageWithAsyncReceipt()
    {
        var opt = TestSetup.AsyncOptions();

        var con = TestSetup.Connection();
        con.SignMessage = true;

        var result = await As4.SendMessage(TestSetup.Input(), con, opt, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.IsReceiptPending, Is.True);
        Assert.That(result.ReceiptStatus, Is.EqualTo("AsyncReceiptPending"));
    }

    [Test]
    public async Task ShouldSendEncryptedMessageWithAsyncReceipt()
    {
        var opt = TestSetup.AsyncOptions();

        var con = TestSetup.Connection();
        con.EncryptMessage = true;

        var result = await As4.SendMessage(TestSetup.Input(), con, opt, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.IsReceiptPending, Is.True);
        Assert.That(result.ReceiptStatus, Is.EqualTo("AsyncReceiptPending"));
    }

    [Test]
    public async Task ShouldFailWithInvalidEndpointUrl()
    {
        var con = TestSetup.Connection();
        con.As4EndpointUrl = "http://invalid-endpoint:9999";

        var opt = TestSetup.Options();
        opt.ThrowErrorOnFailure = false;

        var result = await As4.SendMessage(TestSetup.Input(), con, opt, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        var expectedMessage = OperatingSystem.IsWindows()
            ? "No such host is known"
            : "System error: Resource temporarily unavailable";
        Assert.That(result.Error.Message, Does.Contain(expectedMessage));
    }

    [Test]
    public async Task ShouldFailWithInvalidCertificatePath()
    {
        var con = TestSetup.Connection();
        con.SenderCertificatePath = "invalid/path/sender.pfx";
        con.SignMessage = true;

        var opt = TestSetup.Options();
        opt.ThrowErrorOnFailure = false;

        var result = await As4.SendMessage(TestSetup.Input(), con, opt, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        var expectedMessage = OperatingSystem.IsWindows()
            ? "Cannot open certificate store: The system cannot find the file specified"
            : "The storeName value was invalid.";
        Assert.That(result.Error.Message, Does.Contain(expectedMessage));
    }

    [Test]
    public async Task ShouldFailWithInvalidMessageFilePath()
    {
        var input = TestSetup.Input();
        input.MessageFilePath = "invalid/path/message.txt";

        var opt = TestSetup.Options();
        opt.ThrowErrorOnFailure = false;

        var result = await As4.SendMessage(input, TestSetup.Connection(), opt, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Could not find a part of the path"));
    }
}
