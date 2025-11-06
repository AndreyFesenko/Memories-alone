# Fix-Csproj-Packages.ps1 (robust)
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ver = @{
  "AspNetCore.HealthChecks.NpgSql"                   = "9.0.0"
  "AspNetCore.HealthChecks.Rabbitmq"                 = "9.0.0"
  "AspNetCore.HealthChecks.Redis"                    = "9.0.0"
  "AspNetCoreRateLimit"                              = "5.0.0"
  "AutoMapper"                                       = "15.0.1"
  "AWSSDK.S3"                                        = "4.0.7.4"
  "BCrypt.Net-Next"                                  = "4.0.3"
  "coverlet.collector"                               = "6.0.4"
  "FluentAssertions"                                 = "8.6.0"
  "FluentValidation"                                 = "12.0.0"
  "FluentValidation.AspNetCore"                      = "11.3.1"
  "FluentValidation.DependencyInjectionExtensions"   = "12.0.0"
  "Handlebars.Net"                                   = "2.1.6"
  "MailKit"                                          = "4.13.0"
  "MassTransit"                                      = "8.5.3"
  "MassTransit.AspNetCore"                           = "7.3.1"
  "MassTransit.RabbitMQ"                             = "8.5.3"
  "MediatR"                                          = "13.0.0"
  "Microsoft.AspNetCore.Authentication.JwtBearer"    = "9.0.9"
  "Microsoft.AspNetCore.Http"                        = "2.3.0"
  "Microsoft.AspNetCore.Mvc.NewtonsoftJson"          = "9.0.9"
  "Microsoft.AspNetCore.OpenApi"                     = "9.0.9"
  "Microsoft.AspNetCore.SignalR"                     = "1.2.0"
  "Microsoft.AspNetCore.SignalR.Protocols.Json"      = "9.0.9"
  "Microsoft.AspNetCore.SignalR.StackExchangeRedis"  = "9.0.9"
  "Microsoft.EntityFrameworkCore"                    = "9.0.9"
  "Microsoft.EntityFrameworkCore.Design"             = "9.0.9"
  "Microsoft.EntityFrameworkCore.Relational"         = "9.0.9"
  "Microsoft.EntityFrameworkCore.SqlServer"          = "9.0.9"
  "Microsoft.EntityFrameworkCore.Tools"              = "9.0.9"
  "Microsoft.Extensions.Caching.StackExchangeRedis"  = "9.0.9"
  "Microsoft.Extensions.Configuration"               = "9.0.9"
  "Microsoft.Extensions.Configuration.Binder"        = "9.0.9"
  "Microsoft.Extensions.Configuration.FileExtensions"= "9.0.9"
  "Microsoft.Extensions.Configuration.Json"          = "9.0.9"
  "Microsoft.Extensions.DependencyInjection"         = "9.0.9"
  "Microsoft.Extensions.DependencyInjection.Abstractions" = "9.0.9"
  "Microsoft.Extensions.Hosting"                     = "9.0.9"
  "Microsoft.Extensions.Logging"                     = "9.0.9"
  "Microsoft.Extensions.Options.ConfigurationExtensions" = "9.0.9"
  "Microsoft.IdentityModel.Tokens"                   = "8.14.0"
  "Microsoft.NET.Test.Sdk"                           = "17.14.1"
  "Minio"                                            = "6.0.5"
  "Moq"                                              = "4.20.72"
  "Npgsql"                                           = "9.0.3"
  "Npgsql.EntityFrameworkCore.PostgreSQL"            = "9.0.4"
  "QRCoder"                                          = "1.6.0"
  "RabbitMQ.Client"                                  = "7.1.2"
  "Scalar.AspNetCore"                                = "2.8.6"
  "SendGrid"                                         = "9.29.3"
  "Serilog"                                          = "4.3.0"
  "Serilog.AspNetCore"                               = "9.0.0"
  "Serilog.Sinks.Console"                            = "6.0.0"
  "Serilog.Sinks.File"                               = "7.0.0"
  "Serilog.Sinks.Seq"                                = "9.0.0"
  "Swashbuckle.AspNetCore"                           = "9.0.4"
  "Swashbuckle.AspNetCore.Annotations"               = "9.0.4"
  "Swashbuckle.AspNetCore.Swagger"                   = "9.0.4"
  "Swashbuckle.AspNetCore.SwaggerGen"                = "9.0.4"
  "Swashbuckle.AspNetCore.SwaggerUI"                 = "9.0.4"
  "System.ComponentModel.Annotations"                = "5.0.0"
  "System.IdentityModel.Tokens.Jwt"                  = "8.14.0"
  "xunit"                                            = "2.9.3"
  "xunit.runner.visualstudio"                        = "3.1.4"
  "Yarp.ReverseProxy"                                = "2.3.0"
}

$csprojs = Get-ChildItem -Recurse -Filter *.csproj | Where-Object {
  $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\'
}
if (-not $csprojs) { Write-Host "csproj не найдены"; exit 1 }

foreach ($proj in $csprojs) {
  Write-Host ">>> $($proj.FullName)"
  [xml]$xml = Get-Content $proj.FullName -Raw
  $changed = $false

  # ---- Ensure <PropertyGroup><ManagePackageVersionsCentrally>false</...>
  $pg = $xml.Project.SelectSingleNode('PropertyGroup')
  if (-not $pg) {
    $pg = $xml.CreateElement('PropertyGroup')
    $null = $xml.Project.AppendChild($pg)
    $changed = $true
  }
  $cpm = $pg.SelectSingleNode('ManagePackageVersionsCentrally')
  if ($cpm) {
    if ($cpm.InnerText -ne 'false') {
      $cpm.InnerText = 'false'
      $changed = $true
    }
  } else {
    $cpm = $xml.CreateElement('ManagePackageVersionsCentrally')
    $cpm.InnerText = 'false'
    $null = $pg.AppendChild($cpm)
    $changed = $true
  }

  # ---- Iterate PackageReference and set versions
  $packageRefs = $xml.Project.SelectNodes('//PackageReference')
  foreach ($pr in $packageRefs) {
    $name = $pr.GetAttribute('Include')
    if ([string]::IsNullOrWhiteSpace($name)) { continue }

    # remove VersionOverride attr
    if ($pr.HasAttribute('VersionOverride')) {
      $pr.RemoveAttribute('VersionOverride')
      $changed = $true
    }
    # remove nested <Version>...</Version>
    $verNode = $pr.SelectSingleNode('Version')
    if ($verNode) {
      [void]$pr.RemoveChild($verNode)
      $changed = $true
    }

    $target = $ver[$name]
    if ($target) {
      if (-not $pr.HasAttribute('Version') -or $pr.GetAttribute('Version') -ne $target) {
        $pr.SetAttribute('Version', $target)
        Write-Host ("  {0} -> {1}" -f $name, $target)
        $changed = $true
      }
    }
  }

  if ($changed) { $xml.Save($proj.FullName); Write-Host "  Saved" } else { Write-Host "  No changes" }
}
