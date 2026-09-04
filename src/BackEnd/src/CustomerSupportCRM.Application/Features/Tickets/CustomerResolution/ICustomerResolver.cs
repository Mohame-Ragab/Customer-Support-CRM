namespace CustomerSupportCRM.Application.Features.Tickets.CustomerResolution;

/// <summary>
/// Shared by the two F03 intake channels that receive a submitter identity but
/// not an existing Customer id (web-forms: anonymous submitter; live-chat: an
/// authenticated Customer-role ApplicationUser, which is deliberately NOT
/// linked to the Customer CRM entity - see Customer.cs's own TODO comment).
/// Resolves an existing Customer CRM record by email (case-insensitive), or
/// creates one, so both channels can call the existing CreateTicketCommand
/// (which requires a CustomerId) without duplicating this lookup/creation logic.
/// </summary>
public interface ICustomerResolver
{
    Task<Guid> ResolveOrCreateByEmailAsync(
        string email, string? displayName, CancellationToken cancellationToken);

    /// <summary>
    /// The authenticated-caller counterpart to <see cref="ResolveOrCreateByEmailAsync"/>:
    /// resolves by <c>Customer.ApplicationUserId</c> first (the reliable,
    /// FK-based match), falls back to an email match and backfills the link
    /// onto it if found, or creates a new linked Customer otherwise. Used by
    /// every caller that has an authenticated ApplicationUser (registration,
    /// portal ticket submission, live chat) - callers with only an anonymous
    /// submitter identity (web-forms) keep using the email-only overload.
    /// </summary>
    Task<Guid> ResolveForApplicationUserAsync(
        Guid applicationUserId, string email, string? displayName, CancellationToken cancellationToken);
}
