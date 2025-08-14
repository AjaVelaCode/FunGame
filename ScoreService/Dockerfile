# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ScoreService.csproj", "."]
COPY ["../FunGame.Common/FunGame.Common.csproj", "FunGame.Common/"]
RUN dotnet restore "./ScoreService.csproj"
COPY . .
COPY ../FunGame.Common/ ./FunGame.Common/
WORKDIR "/src/."
RUN dotnet build "ScoreService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScoreService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY nlog.config .
ENTRYPOINT ["dotnet", "ScoreService.dll"]