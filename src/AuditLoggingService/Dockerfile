
# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем весь код (чтобы были Shared.Messaging и другие проекты)
COPY . .

# Переходим в директорию API
WORKDIR /src/AuditLoggingService/AuditLoggingService.API

# Восстановление зависимостей
RUN dotnet restore "AuditLoggingService.API.csproj"

# Сборка
RUN dotnet build "AuditLoggingService.API.csproj" -c Release -o /app/build

# Публикация
RUN dotnet publish "AuditLoggingService.API.csproj" -c Release -o /app/publish

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuditLoggingService.API.dll"]
