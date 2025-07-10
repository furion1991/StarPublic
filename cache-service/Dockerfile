# Базовый рантайм
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7008

# Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY NuGet.Config ./

# Копируем проект
COPY ./cache-service.csproj ./
RUN dotnet restore "./cache-service.csproj" --configfile ./NuGet.Config

# Копируем исходники
COPY . .

RUN dotnet build "cache-service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "cache-service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный рантайм
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cache-service.dll"]
