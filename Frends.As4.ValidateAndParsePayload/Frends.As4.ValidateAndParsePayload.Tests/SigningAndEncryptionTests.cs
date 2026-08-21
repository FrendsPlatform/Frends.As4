using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Frends.As4.ValidateAndParsePayload.Tests;

/// <summary>
/// Tests that exercise signing-only, encryption-only, and combined signing+encryption
/// scenarios using pre-built static AS4 messages and certificates stored in TestData.
/// </summary>
[TestFixture]
internal class SigningAndEncryptionTests : TestBase
{
    [Test]
    public async Task Should_Parse_Signed_As4_Message()
    {
        var result = await As4.ValidateAndParsePayload(
            LoadMessage("signed_message"),
            SignedConnection(),
            DefaultOptions(),
            CancellationToken.None);

        AssertSuccess(result);
    }

    [Test]
    public async Task Should_Parse_Encrypted_As4_Message()
    {
        var result = await As4.ValidateAndParsePayload(
            LoadMessage("encrypted_message"),
            EncryptedConnection(),
            DefaultOptions(),
            CancellationToken.None);

        AssertSuccess(result);
    }

    [Test]
    public async Task Should_Parse_Signed_And_Encrypted_As4_Message()
    {
        var result = await As4.ValidateAndParsePayload(
            LoadMessage("signed_encrypted_message"),
            SignedAndEncryptedConnection(),
            DefaultOptions(),
            CancellationToken.None);

        AssertSuccess(result);
    }
}
