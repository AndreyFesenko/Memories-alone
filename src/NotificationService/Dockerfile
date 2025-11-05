FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/NotificationService/NotificationService.API
RUN dotnet restore "NotificationService.API.csproj"
RUN dotnet publish "NotificationService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["bash","-lc","dotnet NotificationService.API.dll --urls http://0.0.0.0:$PORT"]
