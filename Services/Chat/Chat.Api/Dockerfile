FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Chat/Chat.Api/Chat.Api.csproj", "Services/Chat/Chat.Api/"]
COPY ["Services/Chat/Chat.Application/Chat.Application.csproj", "Services/Chat/Chat.Application/"]
COPY ["Services/Chat/Chat.Domain/Chat.Domain.csproj", "Services/Chat/Chat.Domain/"]
COPY ["Services/Chat/Chat.Infrastructure/Chat.Infrastructure.csproj", "Services/Chat/Chat.Infrastructure/"]
RUN dotnet restore "Services/Chat/Chat.Api/Chat.Api.csproj"
COPY . .
WORKDIR "/src/Services/Chat/Chat.Api"
RUN dotnet build "Chat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chat.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chat.Api.dll"]
