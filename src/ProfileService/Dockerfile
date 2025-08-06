FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/ProfileService/ProfileService.API
RUN dotnet restore "ProfileService.API.csproj"
RUN dotnet build "ProfileService.API.csproj" -c Release -o /app/build
RUN dotnet publish "ProfileService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProfileService.API.dll"]
