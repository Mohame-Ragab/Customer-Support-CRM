using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Commands;

/// <summary>
/// Soft-deletes via the generic repository (BaseEntity.IsDeleted), not by
/// flipping IsPublished - those are distinct states (a published item can be
/// unpublished without deleting it, and vice versa). Idempotent: deleting an
/// already-deleted (or never-existent) id still succeeds, per the plan's
/// explicit "delete of already-deleted item returns 204" edge case - the two
/// cases are indistinguishable once the soft-delete filter excludes the row.
/// </summary>
public sealed class DeleteKnowledgeBaseContentCommandHandler : IRequestHandler<DeleteKnowledgeBaseContentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteKnowledgeBaseContentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteKnowledgeBaseContentCommand request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Repository<KnowledgeBaseContent>().GetByIdAsync(request.Id, cancellationToken);
        if (content is null)
        {
            return;
        }

        _unitOfWork.Repository<KnowledgeBaseContent>().Delete(content);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
