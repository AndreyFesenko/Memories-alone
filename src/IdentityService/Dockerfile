FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/IdentityService/IdentityService.API
RUN dotnet restore "IdentityService.API.csproj"
RUN dotnet build "IdentityService.API.csproj" -c Release -o /app/build
RUN dotnet publish "IdentityService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]
