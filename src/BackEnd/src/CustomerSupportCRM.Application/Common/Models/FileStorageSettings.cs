namespace CustomerSupportCRM.Application.Common.Models;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    /// <summary>Absolute or content-root-relative directory for local storage.</summary>
    public string LocalRootPath { get; set; } = "App_Data/attachments";

    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB

    public string[] AllowedExtensions { get; set; } =
    {
        ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".webp",
        ".txt", ".csv", ".doc", ".docx", ".xls", ".xlsx",
    };
}
