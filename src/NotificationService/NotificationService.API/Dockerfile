FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем всё из src/NotificationService
COPY ../../NotificationService ./NotificationService

WORKDIR /src/NotificationService/NotificationService.API

RUN dotnet restore "NotificationService.API.csproj"
RUN dotnet build "NotificationService.API.csproj" -c Release -o /app/build
RUN dotnet publish "NotificationService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
