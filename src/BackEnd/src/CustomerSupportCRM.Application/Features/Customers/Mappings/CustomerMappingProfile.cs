using AutoMapper;
using CustomerSupportCRM.Application.Features.Customers.Dtos;
using CustomerSupportCRM.Domain.Entities;

namespace CustomerSupportCRM.Application.Features.Customers.Mappings;

public sealed class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerDto>();
    }
}
