FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

WORKDIR /src/MemoryArchiveService/MemoryArchiveService.API
RUN dotnet restore "MemoryArchiveService.API.csproj"
RUN dotnet build "MemoryArchiveService.API.csproj" -c Release -o /app/build
RUN dotnet publish "MemoryArchiveService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MemoryArchiveService.API.dll"]
