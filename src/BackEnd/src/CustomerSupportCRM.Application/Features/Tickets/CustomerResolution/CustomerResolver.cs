using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;

namespace CustomerSupportCRM.Application.Features.Tickets.CustomerResolution;

public sealed class CustomerResolver : ICustomerResolver
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerResolver(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ResolveOrCreateByEmailAsync(
        string email, string? displayName, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();

        var existing = await _unitOfWork.Repository<Customer>()
            .FindAsync(c => c.Email.ToLower() == normalizedEmail.ToLower(), cancellationToken);

        var match = existing.FirstOrDefault();
        if (match is not null)
        {
            return match.Id;
        }

        var (firstName, lastName) = SplitDisplayName(displayName, normalizedEmail);

        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = normalizedEmail,
        };

        await _unitOfWork.Repository<Customer>().AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }

    public async Task<Guid> ResolveForApplicationUserAsync(
        Guid applicationUserId, string email, string? displayName, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Customer>();

        var byUserId = await repository.FindAsync(c => c.ApplicationUserId == applicationUserId, cancellationToken);
        var linked = byUserId.FirstOrDefault();
        if (linked is not null)
        {
            return linked.Id;
        }

        var normalizedEmail = email.Trim();

        // Deterministic backfill: a Customer row with this exact email already
        // exists (e.g. created earlier by staff, or by an anonymous web-forms
        // submission before this person ever registered a portal account) -
        // link it rather than creating a duplicate. Safe because Customer.Email
        // and ApplicationUser.Email are each unique within their own table, so
        // this is a 1:1 match, never a guess.
        var byEmail = await repository.FindAsync(
            c => c.Email.ToLower() == normalizedEmail.ToLower(), cancellationToken);
        var emailMatch = byEmail.FirstOrDefault();
        if (emailMatch is not null)
        {
            emailMatch.ApplicationUserId = applicationUserId;
            repository.Update(emailMatch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return emailMatch.Id;
        }

        var (firstName, lastName) = SplitDisplayName(displayName, normalizedEmail);

        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = normalizedEmail,
            ApplicationUserId = applicationUserId,
        };

        await repository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }

    private static (string FirstName, string LastName) SplitDisplayName(string? displayName, string email)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return (email, string.Empty);
        }

        var parts = displayName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], string.Empty);
    }
}
