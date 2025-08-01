# Билд-образ
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["APIGateway/APIGateway.csproj", "APIGateway/"]
RUN dotnet restore "APIGateway/APIGateway.csproj"

COPY . .
WORKDIR "/src/APIGateway"
RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"]
