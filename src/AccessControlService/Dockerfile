FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/AccessControlService/AccessControlService.API
RUN dotnet restore "AccessControlService.API.csproj"
RUN dotnet build "AccessControlService.API.csproj" -c Release -o /app/build
RUN dotnet publish "AccessControlService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AccessControlService.API.dll"]
