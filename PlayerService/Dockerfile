# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PlayerService/PlayerService.csproj", "PlayerService/"]
COPY ["Common/FunGame.Common.csproj", "Common/"]
RUN dotnet restore "PlayerService/PlayerService.csproj"
COPY PlayerService/ PlayerService/
COPY Common/ Common/
WORKDIR "/src/PlayerService"
RUN dotnet build "PlayerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlayerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY PlayerService/nlog.config .
RUN mkdir -p /app/logs
ENTRYPOINT ["dotnet", "PlayerService.dll"]