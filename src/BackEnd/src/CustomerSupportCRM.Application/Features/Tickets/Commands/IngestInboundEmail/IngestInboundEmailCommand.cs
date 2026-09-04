using CustomerSupportCRM.Application.Common.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.IngestInboundEmail;

public sealed record IngestInboundEmailCommand(InboundEmail Email) : IRequest<Guid?>;
