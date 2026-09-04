using AutoMapper;
using CustomerSupportCRM.Application.Features.Chat.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Queries;

public sealed class GetOpenChatSessionsQueryHandler
    : IRequestHandler<GetOpenChatSessionsQuery, IReadOnlyList<ChatSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOpenChatSessionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<IReadOnlyList<ChatSessionDto>> Handle(
        GetOpenChatSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _unitOfWork.Repository<ChatSession>().Query()
            .Where(s => s.EndedAtUtc == null && s.AssignedAgentUserId == null)
            .OrderBy(s => s.StartedAtUtc)
            .ToList();

        IReadOnlyList<ChatSessionDto> result = _mapper.Map<List<ChatSessionDto>>(sessions);
        return Task.FromResult(result);
    }
}
