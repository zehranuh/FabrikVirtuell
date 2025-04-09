# Verwende das Basis-Image für ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Verwende das Basis-Image für das Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RestAPI/RestAPI.csproj", "RestAPI/"]
RUN dotnet restore "RestAPI/RestAPI.csproj"
COPY . .
WORKDIR "/src/RestAPI"
RUN dotnet build "RestAPI.csproj" -c Release -o /app/build

# Verwende das Basis-Image für die Veröffentlichung
FROM build AS publish
RUN dotnet publish "RestAPI.csproj" -c Release -o /app/publish

# Verwende das Basis-Image für die Ausführung
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestAPI.dll"]