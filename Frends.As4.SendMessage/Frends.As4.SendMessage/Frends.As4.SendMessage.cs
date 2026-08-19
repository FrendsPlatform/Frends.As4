using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Frends.As4.SendMessage.Definitions;
using Frends.As4.SendMessage.Helpers;

namespace Frends.As4.SendMessage;

/// <summary>
/// Task class
/// </summary>
public static class As4
{
    /// <summary>
    /// As4 SendMessage task sends a message to an AS4 server using the provided parameters.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-As4-SendMessage)
    /// </summary>
    /// <param name="input">Essential parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, string PartnerResponse, string MessageId, bool IsReceiptPending, string ReceiptStatus, string ReceiptMessage, string ReceiptReferenceMessageId, object Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> SendMessage(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            ValidationHandler.Run(input, connection, options);

            var as4 = NSoftware.Activation.NSoftware.ActivateAs4Client();
            As4Handler.ConfigureMessage(as4, input, connection, options);
            As4Handler.ConfigureSecurity(as4, connection);
            As4Handler.ConfigurePayload(as4, input, connection, cancellationToken);
            As4Handler.ConfigureLogging(as4, options);

            await as4.SendFiles(cancellationToken);

            return As4Handler.CreateResult(as4, options);
        }
        catch (Exception e)
        {
            return ErrorHandler.Handle(e, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }
}
