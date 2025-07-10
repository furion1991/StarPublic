# Базовый образ для рантайма
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 9000

# Образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем nuget.config (обязательно!)
COPY NuGet.Config ./

# Копируем csproj
COPY ./ApiGateway.csproj ./
RUN dotnet restore "./ApiGateway.csproj" --configfile ./NuGet.Config

# Копируем остальной код
COPY . .
RUN dotnet build "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build


# Публикуем
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный рантайм-слой
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
