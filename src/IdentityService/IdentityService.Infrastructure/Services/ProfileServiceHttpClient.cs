using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using System.Net.Http.Json;

namespace IdentityService.Infrastructure.Services;

public class 
    ProfileServiceHttpClient : IProfileServiceClient
{
    private readonly HttpClient _client;

    public ProfileServiceHttpClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Пример: /api/profile/{userId}
        var response = await _client.GetAsync($"/api/profile/{userId}", cancellationToken);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserProfileDto>(cancellationToken: cancellationToken);
        return null;
    }
}
