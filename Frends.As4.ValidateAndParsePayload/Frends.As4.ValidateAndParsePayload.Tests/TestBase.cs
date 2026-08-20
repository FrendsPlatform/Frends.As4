using System.Collections.Generic;
using Frends.As4.ValidateAndParsePayload.Definitions;

namespace Frends.As4.ValidateAndParsePayload.Tests;

internal abstract class TestBase
{
    protected const string DefaultErrorMessage = "Error occurred";

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
}
