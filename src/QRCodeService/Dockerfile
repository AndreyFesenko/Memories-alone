# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем весь репозиторий (нужно для ProjectReference)
COPY . .

# ВАЖНО: путь к csproj от корня репо
ARG PROJECT_REL=src/QRCodeService/QRCodeService.API.csproj

RUN dotnet restore "$PROJECT_REL"
RUN dotnet publish "$PROJECT_REL" -c Release -o /app/publish /p:UseAppHost=false

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "QRCodeService.API.dll"]
