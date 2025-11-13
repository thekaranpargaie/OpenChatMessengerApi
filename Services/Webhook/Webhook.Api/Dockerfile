FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Webhook/Webhook.Api/Webhook.Api.csproj", "Services/Webhook/Webhook.Api/"]
RUN dotnet restore "Services/Webhook/Webhook.Api/Webhook.Api.csproj"
COPY . .
WORKDIR "/src/Services/Webhook/Webhook.Api"
RUN dotnet build "Webhook.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Webhook.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webhook.Api.dll"]
