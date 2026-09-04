using AutoMapper;
using CustomerSupportCRM.Application.Common.Exceptions;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Tickets;
using CustomerSupportCRM.Application.Features.Tickets.Commands.AssignTicket;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Application.Features.Tickets.History;
using CustomerSupportCRM.Domain.Constants;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

/// <summary>
/// Lives in Infrastructure (not Application, unlike its sibling F02 command
/// handlers) because verifying the assignment target actually holds the Agent
/// role requires <see cref="UserManager{TUser}"/> - an Identity/Infrastructure
/// type Application must not reference. Mirrors the RoleAdminService /
/// UserManagementService precedent (security-admin/manage-roles,
/// security-admin/manage-users).
/// </summary>
public sealed class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITicketHistoryWriter _historyWriter;
    private readonly IMapper _mapper;

    public AssignTicketCommandHandler(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        ITicketHistoryWriter historyWriter,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _historyWriter = historyWriter;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketRepository = _unitOfWork.Repository<Ticket>();
        var ticket = await ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        var targetUser = await _userManager.FindByIdAsync(request.AgentUserId.ToString());
        var isAgent = targetUser != null && await _userManager.IsInRoleAsync(targetUser, Roles.Agent);

        if (!isAgent)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.AgentUserId), "The selected user is not an Agent."),
            });
        }

        var oldAgentId = ticket.AssignedAgentId;
        if (oldAgentId == request.AgentUserId)
        {
            // Idempotent no-op: same-agent reassignment succeeds without a
            // new audit row or history entry.
            await ticket.AttachCategoryIfNeededAsync(_unitOfWork, cancellationToken);
            return _mapper.Map<TicketDto>(ticket);
        }

        var now = DateTime.UtcNow;
        ticket.AssignedAgentId = request.AgentUserId;
        ticket.AssignedAt = now;
        ticketRepository.Update(ticket);

        var assignedByUserId = _currentUserService.UserId ?? Guid.Empty;
        await _unitOfWork.Repository<TicketAssignment>().AddAsync(
            new TicketAssignment
            {
                TicketId = ticket.Id,
                AssignedAgentId = request.AgentUserId,
                AssignedByUserId = assignedByUserId,
                AssignedAtUtc = now,
            },
            cancellationToken);

        await _historyWriter.AppendAsync(
            ticket.Id, TicketHistoryEventType.AssignmentChanged,
            oldValue: oldAgentId?.ToString(), newValue: request.AgentUserId.ToString(),
            note: null, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await ticket.AttachCategoryIfNeededAsync(_unitOfWork, cancellationToken);
        return _mapper.Map<TicketDto>(ticket);
    }
}
