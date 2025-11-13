FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UI/ChatUI/ChatUI.csproj", "UI/ChatUI/"]
RUN dotnet restore "UI/ChatUI/ChatUI.csproj"
COPY . .
WORKDIR "/src/UI/ChatUI"
RUN dotnet build "ChatUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatUI.dll"]
