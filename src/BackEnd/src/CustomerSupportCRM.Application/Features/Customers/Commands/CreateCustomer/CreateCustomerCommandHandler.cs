using AutoMapper;
using CustomerSupportCRM.Application.Features.Customers.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Customer>();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var exists = await repository.ExistsAsync(c => c.Email == normalizedEmail, cancellationToken);
        if (exists)
        {
            throw new ConflictException("A customer with this email already exists.");
        }

        var customer = new Customer
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            PhoneNumber = request.PhoneNumber?.Trim(),
            CompanyName = request.CompanyName?.Trim(),
            PreferredLanguage = request.PreferredLanguage?.Trim(),
            Notes = request.Notes?.Trim(),
        };

        await repository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerDto>(customer);
    }
}
