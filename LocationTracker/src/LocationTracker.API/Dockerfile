FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
USER app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/LocationTracker.API/LocationTracker.API.csproj", "src/LocationTracker.API/"]
COPY ["src/LocationTracker.Domain/LocationTracker.Domain.csproj", "src/LocationTracker.Domain/"]
COPY ["src/LocationTracker.Infrastructure/LocationTracker.Infrastructure.csproj", "src/LocationTracker.Infrastructure/"]
COPY ["src/LocationTracker.Core/LocationTracker.Core.csproj", "src/LocationTracker.Core/"]
RUN dotnet restore "src/LocationTracker.API/LocationTracker.API.csproj"
COPY . .
WORKDIR "/src/src/LocationTracker.API"
RUN dotnet build "./LocationTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LocationTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LocationTracker.API.dll"]
