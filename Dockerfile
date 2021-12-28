# NuGet restore
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /restore
COPY *.sln .
COPY Source/BlazorApp.ApiInfrastructure/*.csproj Source/BlazorApp.ApiInfrastructure/
COPY Source/BlazorApp.IdentityInfrastructure/*.csproj Source/BlazorApp.IdentityInfrastructure/
COPY Source/BlazorApp.CommonInfrastructure/*.csproj Source/BlazorApp.CommonInfrastructure/
COPY Source/BlazorApp.Client/*.csproj Source/BlazorApp.Client/
COPY Source/BlazorApp.Application/*.csproj Source/BlazorApp.Application/
COPY Source/BlazorApp.Domain/*.csproj Source/BlazorApp.Domain/
COPY Source/BlazorApp.Shared/*.csproj Source/BlazorApp.Shared/
COPY Source/BlazorApp.Host/*.csproj Source/BlazorApp.Host/
RUN dotnet restore
COPY . .

# publish
FROM build AS publish
WORKDIR /restore/Source/BlazorApp.Host
RUN dotnet publish -c Release -o /output

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=publish /output .
# ENTRYPOINT ["dotnet", "BlazorApp.Host.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet BlazorApp.Host.dll