using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A single FAQ / Article / Solution-Guide item (F06 knowledge-base/manage-knowledge-base-content).
/// </summary>
/// <remarks>
/// TODO(STORY-045): bilingual authoring is currently one row per language (see
/// <see cref="Language"/>); if product decides authors must pair EN/AR
/// translations, add a <c>GroupId</c> linking rows instead of changing this shape.
/// TODO(STORY-045): no customer-authored content or editorial workflow in v1 -
/// <see cref="IsPublished"/> is a simple boolean, not a multi-step approval flow.
/// </remarks>
public class KnowledgeBaseContent : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public ContentType Type { get; set; }

    /// <summary>ISO 639-1 code: "en" | "ar".</summary>
    public string Language { get; set; } = "en";

    public bool IsPublished { get; set; } = true;
}
