using IdentityService.Application.Interfaces;
using IdentityService.Application.DTOs; 
using System;
using System.Threading;
using System.Threading.Tasks;

public class ProfileServiceStub : IProfileServiceClient
{
    public Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<UserProfileDto?>(new UserProfileDto
        {
            UserId = userId,
            AccessMode = "AfterDeath",
            DeathConfirmed = false
        });
    }
}