using AutoMapper;
using CustomerSupportCRM.Application.Features.Chat.Dtos;
using CustomerSupportCRM.Domain.Entities;

namespace CustomerSupportCRM.Application.Features.Chat.Mappings;

public sealed class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<ChatSession, ChatSessionDto>();
        CreateMap<ChatMessage, ChatMessageDto>();
    }
}
