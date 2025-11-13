FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Archive/Archive.Api/Archive.Api.csproj", "Services/Archive/Archive.Api/"]
RUN dotnet restore "Services/Archive/Archive.Api/Archive.Api.csproj"
COPY . .
WORKDIR "/src/Services/Archive/Archive.Api"
RUN dotnet build "Archive.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Archive.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Archive.Api.dll"]
