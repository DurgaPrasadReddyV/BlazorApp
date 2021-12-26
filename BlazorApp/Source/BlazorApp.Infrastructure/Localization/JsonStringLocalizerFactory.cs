using BlazorApp.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;

namespace BlazorApp.Infrastructure.Localization;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{

    public IStringLocalizer Create(Type resourceSource) =>
        new JsonStringLocalizer();

    public IStringLocalizer Create(string baseName, string location) =>
        new JsonStringLocalizer();
}