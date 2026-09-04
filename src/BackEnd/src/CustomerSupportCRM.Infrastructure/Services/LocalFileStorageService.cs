using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Domain.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.Infrastructure.Services;

/// <summary>
/// Local-disk implementation of <see cref="IFileStorageService"/>
/// (customers/manage-customer-attachments). Files are written under
/// &lt;root&gt;/&lt;yyyy&gt;/&lt;MM&gt;/&lt;guid&gt;&lt;ext&gt; - the fresh Guid filename makes
/// filename collisions impossible regardless of the original upload name.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IOptions<FileStorageSettings> settings, IHostEnvironment environment)
    {
        var configuredRoot = settings.Value.LocalRootPath;
        _rootPath = Path.IsPathRooted(configuredRoot)
            ? configuredRoot
            : Path.Combine(environment.ContentRootPath, configuredRoot);
    }

    public async Task<string> SaveAsync(
        Stream content, string suggestedFileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(suggestedFileName);
        var now = DateTime.UtcNow;
        var relativeDir = Path.Combine(now.Year.ToString(), now.Month.ToString("D2"));
        var fileName = $"{Guid.NewGuid()}{extension}";
        var storageKey = Path.Combine(relativeDir, fileName).Replace('\\', '/');

        var absoluteDir = Path.Combine(_rootPath, relativeDir);
        Directory.CreateDirectory(absoluteDir);

        var absolutePath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        await using var fileStream = File.Create(absolutePath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return storageKey;
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey)
            || storageKey.Contains("..", StringComparison.Ordinal)
            || Path.IsPathRooted(storageKey))
        {
            throw new NotFoundException("Attachment file", storageKey);
        }

        var absolutePath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        // Defense in depth: resolve and re-check the path stays under the root
        // even after combining, in case of an encoded traversal sequence.
        var fullRoot = Path.GetFullPath(_rootPath);
        var fullTarget = Path.GetFullPath(absolutePath);
        if (!fullTarget.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotFoundException("Attachment file", storageKey);
        }

        if (!File.Exists(fullTarget))
        {
            throw new NotFoundException("Attachment file", storageKey);
        }

        Stream stream = File.OpenRead(fullTarget);
        return Task.FromResult(stream);
    }
}
