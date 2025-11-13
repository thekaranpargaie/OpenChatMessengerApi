FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Presence/Presence.Api/Presence.Api.csproj", "Services/Presence/Presence.Api/"]
RUN dotnet restore "Services/Presence/Presence.Api/Presence.Api.csproj"
COPY . .
WORKDIR "/src/Services/Presence/Presence.Api"
RUN dotnet build "Presence.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Presence.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presence.Api.dll"]
