# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# �������� ���� ���
COPY . .

# ��������� � ����� API Gateway
WORKDIR /src/APIGateway/APIGateway.API

# ��������������� ����������� � ��������
RUN dotnet restore "APIGateway.API.csproj"
RUN dotnet build "APIGateway.API.csproj" -c Release -o /app/build
RUN dotnet publish "APIGateway.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.API.dll"]
