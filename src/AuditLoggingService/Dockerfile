
# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# �������� ���� ��� (����� ���� Shared.Messaging � ������ �������)
COPY . .

# ��������� � ���������� API
WORKDIR /src/AuditLoggingService/AuditLoggingService.API

# �������������� ������������
RUN dotnet restore "AuditLoggingService.API.csproj"

# ������
RUN dotnet build "AuditLoggingService.API.csproj" -c Release -o /app/build

# ����������
RUN dotnet publish "AuditLoggingService.API.csproj" -c Release -o /app/publish

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuditLoggingService.API.dll"]
