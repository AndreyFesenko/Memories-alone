FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/PublicViewService/PublicViewService.API
RUN dotnet restore "PublicViewService.API.csproj"
RUN dotnet publish "PublicViewService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["bash","-lc","dotnet PublicViewService.API.dll --urls http://0.0.0.0:$PORT"]
