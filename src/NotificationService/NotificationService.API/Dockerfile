FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NotificationService.API/NotificationService.API.csproj", "NotificationService.API/"]
COPY . .
WORKDIR "/src/NotificationService.API"
RUN dotnet restore "NotificationService.API.csproj"
RUN dotnet build "NotificationService.API.csproj" -c Release -o /app/build
RUN dotnet publish "NotificationService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
