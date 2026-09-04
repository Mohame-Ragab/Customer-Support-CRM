using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Notes.Commands.CreateCustomerNote;

public sealed class CreateCustomerNoteCommandHandler : IRequestHandler<CreateCustomerNoteCommand, CustomerNoteDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateCustomerNoteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CustomerNoteDto> Handle(CreateCustomerNoteCommand request, CancellationToken cancellationToken)
    {
        var customerExists = await _unitOfWork.Repository<Customer>()
            .ExistsAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!customerExists)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId);
        }

        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            throw new UnauthorizedException("Authenticated user id is unavailable.");
        }

        var note = new CustomerNote
        {
            CustomerId = request.CustomerId,
            AuthorUserId = userId.Value.ToString(),
            Content = request.Content.Trim(),
        };

        await _unitOfWork.Repository<CustomerNote>().AddAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CustomerNoteDto(note.Id, note.CustomerId, note.AuthorUserId, note.Content, note.CreatedAt);
    }
}
