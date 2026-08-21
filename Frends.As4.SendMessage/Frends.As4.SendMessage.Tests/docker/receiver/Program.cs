using Frends.NSoftware.Activation;
using nsoftware.async.IPWorksEDI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Text("AS4 test receiver is running."));
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/{**path}", async context =>
{
    string? generatedSoap = null;
    var server = NSoftware.ActivateAs4Server();

    server.OnLog += (_, e) =>
    {
        if (e.LogMessage.StartsWith("<?xml version=", StringComparison.Ordinal))
        {
            generatedSoap = e.LogMessage;
        }
    };
    server.OnError += (_, e) => Console.WriteLine($"AS4 error {e.ErrorCode}: {e.Description}");

    ConfigureServer(server);

    context.Request.EnableBuffering();
    await server.SetRequestStream(context.Request.Body);
    context.Request.Body.Position = 0;
    server.RequestHeadersString = string.Join(
        "\r\n",
        context.Request.Headers.Select(header => $"{header.Key}: {header.Value}"));

    try
    {
        await server.ParseRequest();
        Console.WriteLine($"Parsed AS4 message {server.MessageId} with receipt mode {server.ReceiptReplyMode}.");

        if (server.ReceiptReplyMode == AS4ServerReceiptReplyModes.rrmAsync)
        {
            await TryGenerateSoapAsync(server.SendAckResponse);

            if (!string.IsNullOrWhiteSpace(generatedSoap))
            {
                await WriteSoapAsync(context, generatedSoap);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status202Accepted;
            return;
        }

        await TryGenerateSoapAsync(server.SendResponse);

        if (string.IsNullOrWhiteSpace(generatedSoap))
        {
            throw new InvalidOperationException("AS4Server did not expose a SOAP response.");
        }

        await WriteSoapAsync(context, generatedSoap);
        Console.WriteLine("Sent synchronous receipt response.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(ex.ToString());
    }
});

app.Run();

static void ConfigureServer(AS4Server server)
{
    var certsDirectory = ResolveDirectory(
        "/opt/certs",
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "certs"));
    var dataDirectory = ResolveDirectory(
        "/opt/data",
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "data"));
    var receiverCertificatePath = Path.Combine(certsDirectory, "receiver.pfx");
    var senderCertificatePath = Path.Combine(certsDirectory, "sender.pem");
    const string receiverCertificatePassword = "receiver123";

    server.Certificate = CreateCertificate(receiverCertificatePath, receiverCertificatePassword);
    server.SigningCert = CreateCertificate(receiverCertificatePath, receiverCertificatePassword);
    server.SignerCert = new Certificate(senderCertificatePath);

    var incomingDirectory = Path.Combine(dataDirectory, "incoming");
    Directory.CreateDirectory(incomingDirectory);
    server.IncomingDirectory = incomingDirectory;
}

static string ResolveDirectory(params string[] candidates)
{
    foreach (var candidate in candidates)
    {
        var fullPath = Path.GetFullPath(candidate);
        if (Directory.Exists(fullPath))
        {
            return fullPath;
        }
    }

    throw new DirectoryNotFoundException($"Could not resolve any of the expected directories: {string.Join(", ", candidates)}");
}

static Certificate CreateCertificate(string path, string password)
{
    return new Certificate(CertStoreTypes.cstAuto, path, password, "*");
}

static async Task TryGenerateSoapAsync(Func<Task> sender)
{
    try
    {
        await sender();
    }
    catch (IPWorksEDIException ex) when (ex.Message.Contains("There is no HTTP environment", StringComparison.Ordinal))
    {
    }
}

static async Task WriteSoapAsync(HttpContext context, string soap)
{
    context.Response.StatusCode = StatusCodes.Status200OK;
    context.Response.ContentType = "application/soap+xml; charset=utf-8";
    await context.Response.WriteAsync(soap);
}
