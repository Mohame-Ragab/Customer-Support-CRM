using CustomerSupportCRM.Application.Common.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.IngestInboundEmail;

public sealed class IngestInboundEmailCommandHandler : IRequestHandler<IngestInboundEmailCommand, Guid?>
{
    private readonly IInboundEmailProcessor _processor;

    public IngestInboundEmailCommandHandler(IInboundEmailProcessor processor)
    {
        _processor = processor;
    }

    public Task<Guid?> Handle(IngestInboundEmailCommand request, CancellationToken cancellationToken)
        => _processor.ProcessAsync(request.Email, cancellationToken);
}
