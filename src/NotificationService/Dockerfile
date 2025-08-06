FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/NotificationService/NotificationService.API
RUN dotnet restore "NotificationService.API.csproj"
RUN dotnet build "NotificationService.API.csproj" -c Release -o /app/build
RUN dotnet publish "NotificationService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
