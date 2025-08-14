# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GameService/GameService.csproj", "GameService/"]
COPY ["Common/FunGame.Common.csproj", "Common/"]
RUN dotnet restore "GameService/GameService.csproj"
COPY GameService/ GameService/
COPY Common/ Common/
WORKDIR "/src/GameService"
RUN dotnet build "GameService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY GameService/nlog.config .
RUN mkdir -p /app/logs
ENTRYPOINT ["dotnet", "GameService.dll"]