FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/QRCodeService/QRCodeService.API
RUN dotnet restore "QRCodeService.API.csproj"
RUN dotnet publish "QRCodeService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["bash","-lc","dotnet QRCodeService.API.dll --urls http://0.0.0.0:$PORT"]
