# Create-Memories-Services.ps1

$root = "src"

$services = @(
    "MemoryArchiveService",
    "NotificationService",
    "AccessControlService",
    "APIGateway",
    "ModerationService",
    "QRCodeService",
    "AuditLoggingService",
    "PublicViewService"
)

# All services will have API, Application, Domain, Infrastructure
foreach ($svc in $services) {
    $svcRoot = Join-Path $root $svc
    New-Item -Path $svcRoot -ItemType Directory -Force | Out-Null

    $apiProj = "$svc.API"
    $appProj = "$svc.Application"
    $domainProj = "$svc.Domain"
    $infraProj = "$svc.Infrastructure"

    # API
    $apiPath = Join-Path $svcRoot $apiProj
    dotnet new webapi -n $apiProj -o $apiPath
    Add-Content -Path (Join-Path $apiPath "README.md") -Value "# $apiProj`nASP.NET Core Web API for $svc"

    # Application
    $appPath = Join-Path $svcRoot $appProj
    dotnet new classlib -n $appProj -o $appPath
    Add-Content -Path (Join-Path $appPath "README.md") -Value "# $appProj`nApplication Layer"

    # Domain
    $domainPath = Join-Path $svcRoot $domainProj
    dotnet new classlib -n $domainProj -o $domainPath
    Add-Content -Path (Join-Path $domainPath "README.md") -Value "# $domainProj`nDomain Models"

    # Infrastructure
    $infraPath = Join-Path $svcRoot $infraProj
    dotnet new classlib -n $infraProj -o $infraPath
    Add-Content -Path (Join-Path $infraPath "README.md") -Value "# $infraProj`nInfrastructure Layer"

    # Add references
    dotnet add "$apiPath\$apiProj.csproj" reference "$appPath\$appProj.csproj"
    dotnet add "$apiPath\$apiProj.csproj" reference "$domainPath\$domainProj.csproj"
    dotnet add "$apiPath\$apiProj.csproj" reference "$infraPath\$infraProj.csproj"

    dotnet add "$appPath\$appProj.csproj" reference "$domainPath\$domainProj.csproj"
    dotnet add "$infraPath\$infraProj.csproj" reference "$domainPath\$domainProj.csproj"

    # Add README to service root
    Add-Content -Path (Join-Path $svcRoot "README.md") -Value "# $svc`n`n- API`n- Application`n- Domain`n- Infrastructure"
}

Write-Host ""
Write-Host "Services structure created!"
Write-Host ""
Write-Host "Don't forget to add projects to your .sln:"
Write-Host "  dotnet sln add src/*/*/*.csproj"
Write-Host ""
Write-Host "Each service has API, Application, Domain, Infrastructure."
