using BlazorApp.Domain.Common;
using BlazorApp.Shared.FileStorage;

namespace BlazorApp.Application.FileStorage;

public interface IFileStorageService
{
    public Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType)
    where T : class;
}