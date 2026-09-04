using AutoMapper;
using CustomerSupportCRM.Application.Features.Customers.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Customer>();
        var customer = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException(nameof(Customer), request.Id);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (!string.Equals(normalizedEmail, customer.Email, StringComparison.Ordinal))
        {
            var emailExists = await repository.ExistsAsync(
                c => c.Email == normalizedEmail && c.Id != customer.Id, cancellationToken);

            if (emailExists)
            {
                throw new ConflictException("A customer with this email already exists.");
            }
        }

        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.Email = normalizedEmail;
        customer.PhoneNumber = request.PhoneNumber?.Trim();
        customer.CompanyName = request.CompanyName?.Trim();
        customer.PreferredLanguage = request.PreferredLanguage?.Trim();
        customer.Notes = request.Notes?.Trim();

        repository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerDto>(customer);
    }
}
