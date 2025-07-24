# base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ../../../../src/NotificationService/NotificationService.API/NotificationService.API.csproj .
RUN dotnet restore "NotificationService.API.csproj"
COPY ../../../../ .
WORKDIR /src/src/NotificationService/NotificationService.API
RUN dotnet build "NotificationService.API.csproj" -c Release -o /app/build
RUN dotnet publish "NotificationService.API.csproj" -c Release -o /app/publish

# final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
