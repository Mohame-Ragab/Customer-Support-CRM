using AutoMapper;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Entities;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Mappings;

public sealed class KnowledgeBaseMappingProfile : Profile
{
    public KnowledgeBaseMappingProfile()
    {
        CreateMap<KnowledgeBaseContent, KnowledgeBaseContentDto>();
        CreateMap<KnowledgeBaseContent, KnowledgeBaseContentListItemDto>();
    }
}
