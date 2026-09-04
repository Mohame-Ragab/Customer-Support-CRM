using CustomerSupportCRM.Application.Common.Exceptions;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Customers.Attachments.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Commands.UploadCustomerAttachment;

/// <summary>
/// Handles file-size/extension validation inline (not via a separate
/// FluentValidation validator + ValidationFilter): this command carries a raw
/// Stream and is constructed manually in the controller from an IFormFile, so
/// it never passes through MVC model binding for ValidationFilter to
/// intercept - matches how the plan's own handler steps describe the checks.
/// </summary>
public sealed class UploadCustomerAttachmentCommandHandler
    : IRequestHandler<UploadCustomerAttachmentCommand, CustomerAttachmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;
    private readonly FileStorageSettings _settings;

    public UploadCustomerAttachmentCommandHandler(
        IUnitOfWork unitOfWork, IFileStorageService fileStorage, IOptions<FileStorageSettings> settings)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
        _settings = settings.Value;
    }

    public async Task<CustomerAttachmentDto> Handle(UploadCustomerAttachmentCommand request, CancellationToken cancellationToken)
    {
        var customerExists = await _unitOfWork.Repository<Customer>()
            .ExistsAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!customerExists)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId);
        }

        Validate(request);

        var storageKey = await _fileStorage.SaveAsync(
            request.Content, request.FileName, request.ContentType, cancellationToken);

        var attachment = new CustomerAttachment
        {
            CustomerId = request.CustomerId,
            FileName = SanitizeFileName(request.FileName),
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            StorageKey = storageKey,
        };

        await _unitOfWork.Repository<CustomerAttachment>().AddAsync(attachment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(attachment);
    }

    private void Validate(UploadCustomerAttachmentCommand request)
    {
        var failures = new List<ValidationFailure>();

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            failures.Add(new ValidationFailure(nameof(request.FileName), "File name is required."));
        }

        if (request.SizeBytes <= 0)
        {
            failures.Add(new ValidationFailure(nameof(request.SizeBytes), "Uploaded file is empty."));
        }
        else if (request.SizeBytes > _settings.MaxFileSizeBytes)
        {
            failures.Add(new ValidationFailure(
                nameof(request.SizeBytes),
                $"File exceeds the maximum allowed size of {_settings.MaxFileSizeBytes / (1024 * 1024)} MB."));
        }

        var extension = Path.GetExtension(request.FileName);
        if (string.IsNullOrEmpty(extension)
            || !_settings.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            failures.Add(new ValidationFailure(nameof(request.FileName), $"File type '{extension}' is not allowed."));
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName); // strips any directory component
        return name.Length > 260 ? name[..260] : name;
    }

    private static CustomerAttachmentDto ToDto(CustomerAttachment a) => new(
        a.Id, a.CustomerId, a.FileName, a.ContentType, a.SizeBytes, a.CreatedBy, a.CreatedAt);
}
