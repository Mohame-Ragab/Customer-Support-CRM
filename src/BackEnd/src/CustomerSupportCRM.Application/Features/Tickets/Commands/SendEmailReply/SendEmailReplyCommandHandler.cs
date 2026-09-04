using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Options;
using ValidationException = CustomerSupportCRM.Application.Common.Exceptions.ValidationException;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.SendEmailReply;

public sealed class SendEmailReplyCommandHandler : IRequestHandler<SendEmailReplyCommand, TicketMessageDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITicketEmailSender _emailSender;
    private readonly ICurrentUserService _currentUserService;
    private readonly EmailSettings _settings;
    private readonly IMapper _mapper;

    public SendEmailReplyCommandHandler(
        IUnitOfWork unitOfWork,
        ITicketEmailSender emailSender,
        ICurrentUserService currentUserService,
        IOptions<EmailSettings> settings,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _currentUserService = currentUserService;
        _settings = settings.Value;
        _mapper = mapper;
    }

    public async Task<TicketMessageDto> Handle(SendEmailReplyCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(ticket.CustomerId, cancellationToken);
        if (customer is null || string.IsNullOrWhiteSpace(customer.Email))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("CustomerId", "This customer has no email address on file; cannot send an email reply."),
            });
        }

        var token = ComputeReplyToken(ticket.Id, _settings.Inbound.WebhookSecret);
        var replyToAddress = $"ticket+{ticket.Id:N}.{token}@{_settings.ReplyToDomain}";

        var message = TicketMessage.CreateOutbound(
            ticket.Id,
            fromAddress: _settings.FromAddress,
            toAddress: customer.Email,
            subject: request.Subject,
            bodyText: request.BodyText,
            bodyHtml: request.BodyHtml,
            conversationToken: token,
            sentByUserId: _currentUserService.UserId);

        await _unitOfWork.Repository<TicketMessage>().AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var messageIdHeader = $"<ticket-{ticket.Id:N}-{message.Id:N}@{_settings.ReplyToDomain}>";

        var emailMessage = new EmailMessage(
            To: customer.Email,
            Subject: request.Subject,
            BodyText: request.BodyText,
            BodyHtml: request.BodyHtml,
            InReplyTo: null,
            Headers: new Dictionary<string, string>
            {
                ["Reply-To"] = replyToAddress,
                ["Message-ID"] = messageIdHeader,
            });

        var sendResult = await _emailSender.SendAsync(emailMessage, cancellationToken);

        if (sendResult.Success)
        {
            message.DeliveryStatus = Domain.Enums.EmailDeliveryStatus.Sent;
            message.ProviderMessageId = sendResult.ProviderMessageId ?? messageIdHeader;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TicketMessageDto>(message);
        }

        message.DeliveryStatus = Domain.Enums.EmailDeliveryStatus.Failed;
        message.FailureReason = sendResult.Error;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The row is retained for audit (agent can see it failed); the API
        // still surfaces an error to the caller so nothing is silently dropped.
        throw new EmailDeliveryException(
            sendResult.Error ?? "Failed to send email. The message was recorded but not delivered.");
    }

    private static string ComputeReplyToken(Guid ticketId, string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var payload = ticketId.ToByteArray();
        using var hmac = new HMACSHA256(secretBytes);
        var hash = hmac.ComputeHash(payload);
        return Convert.ToHexString(hash)[..12].ToLowerInvariant();
    }
}
