# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/APIGateway/APIGateway.API
RUN dotnet restore "APIGateway.API.csproj"
RUN dotnet publish "APIGateway.API.csproj" -c Release -o /app/publish

# RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# Статика
COPY /src/APIGateway/APIGateway.API/wwwroot ./wwwroot
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "APIGateway.API.dll"]
