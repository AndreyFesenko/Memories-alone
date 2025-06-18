$solutionName = "Memories"
$services = @("IdentityService")
$layers = @("API", "Application", "Domain", "Infrastructure")

# Создание solution и базовой структуры
dotnet new sln -n $solutionName
New-Item -ItemType Directory -Force -Path "src" | Out-Null

foreach ($service in $services) {
    $servicePath = "src\$service"
    New-Item -ItemType Directory -Force -Path $servicePath | Out-Null

    foreach ($layer in $layers) {
        $projectName = "$service.$layer"
        $projectPath = "$servicePath\$projectName"

        if ($layer -eq "API") {
            dotnet new webapi -n $projectName -o $projectPath --no-https
        } else {
            dotnet new classlib -n $projectName -o $projectPath
        }

        dotnet sln add "$projectPath\$projectName.csproj"
    }

    # Ссылки между проектами
    dotnet add "$servicePath\$service.API\$service.API.csproj" reference `
        "$servicePath\$service.Application\$service.Application.csproj", `
        "$servicePath\$service.Infrastructure\$service.Infrastructure.csproj"

    dotnet add "$servicePath\$service.Application\$service.Application.csproj" reference `
        "$servicePath\$service.Domain\$service.Domain.csproj"

    dotnet add "$servicePath\$service.Infrastructure\$service.Infrastructure.csproj" reference `
        "$servicePath\$service.Application\$service.Application.csproj", `
        "$servicePath\$service.Domain\$service.Domain.csproj"
}

# NuGet-пакеты в API
dotnet add src\IdentityService\IdentityService.API package Swashbuckle.AspNetCore
dotnet add src\IdentityService\IdentityService.API package Serilog.AspNetCore
dotnet add src\IdentityService\IdentityService.API package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src\IdentityService\IdentityService.API package MediatR.Extensions.Microsoft.DependencyInjection

# Application
dotnet add src\IdentityService\IdentityService.Application package MediatR
dotnet add src\IdentityService\IdentityService.Application package FluentValidation

# Infrastructure
dotnet add src\IdentityService\IdentityService.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src\IdentityService\IdentityService.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src\IdentityService\IdentityService.Infrastructure package Microsoft.Extensions.Configuration
dotnet add src\IdentityService\IdentityService.Infrastructure package Microsoft.Extensions.DependencyInjection.Abstractions
