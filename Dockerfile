# NuGet restore
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /restore
COPY *.sln .
COPY src/BlazorApp.WebUI/Server/*.csproj src/BlazorApp.WebUI/Server/
COPY src/BlazorApp.WebUI/Client/*.csproj src/BlazorApp.WebUI/Client/
COPY src/BlazorApp.WebUI/Shared/*.csproj src/BlazorApp.WebUI/Shared/
RUN dotnet restore
COPY . .

# publish
FROM build AS publish
WORKDIR /restore/src/BlazorApp.WebUI/Server
RUN dotnet publish -c Release -o /output

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=publish /output .
# ENTRYPOINT ["dotnet", "BlazorApp.WebUI.Server.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet BlazorApp.WebUI.Server.dll