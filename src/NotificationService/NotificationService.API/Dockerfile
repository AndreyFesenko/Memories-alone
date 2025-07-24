# Stage 1: base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Stage 2: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Указываем точный путь к .csproj
COPY ["src/NotificationService/NotificationService.API/NotificationService.API.csproj", "src/NotificationService/NotificationService.API/"]
COPY . .

# Переходим в нужную папку
WORKDIR "/src/src/NotificationService/NotificationService.API"

RUN dotnet restore "NotificationService.API.csproj"
RUN dotnet build "NotificationService.API.csproj" -c Release -o /app/build
RUN dotnet publish "NotificationService.API.csproj" -c Release -o /app/publish

# Stage 3: final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
