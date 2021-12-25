using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Domain.Common;
using BlazorApp.Shared.FileStorage;

namespace BlazorApp.Application.FileStorage;

public interface IFileStorageService : ITransientService
{
    public Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType)
    where T : class;
}