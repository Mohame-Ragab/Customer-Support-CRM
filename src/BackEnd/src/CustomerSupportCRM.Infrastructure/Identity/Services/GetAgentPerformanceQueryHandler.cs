using CustomerSupportCRM.Application.Features.Reports.AgentPerformance;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ValidationException = CustomerSupportCRM.Application.Common.Exceptions.ValidationException;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

/// <summary>
/// Lives in Infrastructure (not Application) because it resolves each agent's
/// display name via <see cref="UserManager{TUser}"/> - same rationale as
/// ListTicketInternalCommentsQueryHandler (F04) and AssignTicketCommandHandler
/// (F02). "Resolved" time-to-resolution is derived from the existing
/// TicketHistoryEntry audit trail (StatusChanged -> "Resolved", first
/// occurrence) rather than adding a new Ticket.ResolvedAt column - avoids an
/// unrelated schema change while reusing data the ticket lifecycle already
/// records (F02 tickets/change-ticket-status).
/// "Assigned in period" uses Ticket.AssignedAt (the authoritative assignment
/// timestamp set by tickets/assign-ticket), not CreatedAt.
/// </summary>
public sealed class GetAgentPerformanceQueryHandler : IRequestHandler<GetAgentPerformanceQuery, AgentPerformanceReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<GetAgentPerformanceQuery> _validator;

    public GetAgentPerformanceQueryHandler(
        IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IValidator<GetAgentPerformanceQuery> validator)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _validator = validator;
    }

    public async Task<AgentPerformanceReportDto> Handle(
        GetAgentPerformanceQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var ticketRepo = _unitOfWork.Repository<Ticket>();

        var assignedTickets = ticketRepo.Query()
            .Where(t => t.AssignedAgentId != null && t.AssignedAt != null
                && t.AssignedAt >= request.From && t.AssignedAt <= request.To)
            .ToList();
        if (request.AgentId.HasValue)
        {
            assignedTickets = assignedTickets.Where(t => t.AssignedAgentId == request.AgentId.Value).ToList();
        }

        var resolvedEntries = _unitOfWork.Repository<TicketHistoryEntry>().Query()
            .Where(h => h.EventType == TicketHistoryEventType.StatusChanged
                && h.NewValue == nameof(TicketStatus.Resolved)
                && h.OccurredAt >= request.From && h.OccurredAt <= request.To)
            .ToList();

        var resolvedAtByTicket = resolvedEntries
            .GroupBy(h => h.TicketId)
            .ToDictionary(g => g.Key, g => g.Min(h => h.OccurredAt));

        var resolvedTicketIds = resolvedAtByTicket.Keys.ToList();
        var resolvedTickets = resolvedTicketIds.Count == 0
            ? new List<Ticket>()
            : ticketRepo.Query().Where(t => resolvedTicketIds.Contains(t.Id)).ToList();
        if (request.AgentId.HasValue)
        {
            resolvedTickets = resolvedTickets.Where(t => t.AssignedAgentId == request.AgentId.Value).ToList();
        }

        var agentIds = assignedTickets
            .Select(t => t.AssignedAgentId!.Value)
            .Concat(resolvedTickets.Where(t => t.AssignedAgentId.HasValue).Select(t => t.AssignedAgentId!.Value))
            .Distinct()
            .ToList();

        var rows = new List<AgentPerformanceRowDto>();
        foreach (var agentId in agentIds)
        {
            var assignedCount = assignedTickets.Count(t => t.AssignedAgentId == agentId);
            var resolvedForAgent = resolvedTickets.Where(t => t.AssignedAgentId == agentId).ToList();

            double? averageResolutionHours = null;
            if (resolvedForAgent.Count > 0)
            {
                var hours = resolvedForAgent.Select(t => (resolvedAtByTicket[t.Id] - t.CreatedAt).TotalHours);
                averageResolutionHours = Math.Round(hours.Average(), 2);
            }

            var user = await _userManager.FindByIdAsync(agentId.ToString());
            var displayName = user?.FullName ?? user?.Email ?? agentId.ToString();

            rows.Add(new AgentPerformanceRowDto(
                agentId, displayName, assignedCount, resolvedForAgent.Count, averageResolutionHours));
        }

        rows = rows.OrderByDescending(r => r.TicketsAssigned).ToList();

        return new AgentPerformanceReportDto(request.From, request.To, rows);
    }
}
