# NuGet restore
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.sln .
COPY src/BlazorApp.WebUI/*.csproj src/BlazorApp.WebUI/
RUN dotnet restore
COPY . .

# publish
FROM build AS publish
WORKDIR src/BlazorApp.WebUI
RUN dotnet publish -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .
# ENTRYPOINT ["dotnet", "BlazorApp.WebUI.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet BlazorApp.WebUI.dll