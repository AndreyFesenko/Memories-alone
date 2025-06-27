using Xunit;
using Moq;
using FluentAssertions;
using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Handlers;
using IdentityService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var profile = new Mock<IProfileServiceClient>();
        var logger = new Mock<ILogger<LoginCommandHandler>>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
        };

        userRepo.Setup(r => r.FindByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userRepo.Setup(r => r.GetUserRolesAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "RegularUser" });

        profile.Setup(p => p.GetProfileAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfileDto { AccessMode = "Anytime", DeathConfirmed = false });

        jwt.Setup(j => j.GenerateToken(user.Id, user.Email, It.IsAny<IEnumerable<string>>(), It.IsAny<Dictionary<string, string>>()))
            .Returns("jwt-token");

        var handler = new LoginCommandHandler(userRepo.Object, jwt.Object, profile.Object, logger.Object, new FakeAuditService());
        var command = new LoginCommand { Email = user.Email, Password = "password" };

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.AccessToken.Should().Be("jwt-token");
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorized_WhenPasswordInvalid()
    {
        // Arrange
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var profile = new Mock<IProfileServiceClient>();
        var logger = new Mock<ILogger<LoginCommandHandler>>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
        };

        userRepo.Setup(r => r.FindByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new LoginCommandHandler(userRepo.Object, jwt.Object, profile.Object, logger.Object, new FakeAuditService());
        var command = new LoginCommand { Email = user.Email, Password = "wrong" };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    public class FakeAuditService : IAuditService
    {
        public Task LogAsync(string action, string details, Guid? userId = null, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
