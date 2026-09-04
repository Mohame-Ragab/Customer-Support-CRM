using AutoMapper;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;

namespace CustomerSupportCRM.Application.Features.Tickets.Mappings;

public sealed class TicketMappingProfile : Profile
{
    public TicketMappingProfile()
    {
        CreateMap<Ticket, TicketDto>()
            .ForCtorParam(nameof(TicketDto.CategoryCode), opt => opt.MapFrom(t => t.Category != null ? t.Category.Code : null));

        CreateMap<TicketCategory, TicketCategoryDto>();

        // F03 email-communication-channel.
        CreateMap<TicketMessage, TicketMessageDto>();
    }
}
