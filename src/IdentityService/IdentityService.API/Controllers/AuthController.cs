using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IdentityService.Application.Interfaces;

namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRefreshTokenService _refreshTokens;

        public AuthController(IRefreshTokenService refreshTokens, IMediator mediator)
        {
            _refreshTokens = refreshTokens;
            _mediator = mediator;
        }

        /// <summary>
        /// Вход пользователя (login)
        /// </summary>
        /// <remarks>
        /// Введите email и пароль для получения JWT и refresh token.
        /// </remarks>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Вход пользователя", Description = "Выполняет вход и выдаёт access/refresh токены")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <remarks>
        /// После регистрации пользователь получает свой Id и email.
        /// </remarks>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Регистрация нового пользователя")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Обновить токен доступа по refresh-токену
        /// </summary>
        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Обновить access token по refresh")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Logout (инвалидирует refresh-токен)
        /// </summary>
        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Выход (logout)")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand cmd)
        {
            await _mediator.Send(cmd);
            return Ok(new { message = "Logged out" });
        }

        /// <summary>
        /// Запросить подтверждение email
        /// </summary>
        [HttpPost("request-confirmation")]
        [SwaggerOperation(Summary = "Запросить подтверждение email")]
        public async Task<IActionResult> RequestConfirmation([FromBody] RequestEmailConfirmationCommand cmd, CancellationToken ct)
        {
            await _mediator.Send(cmd, ct);
            return Ok(new { message = "Confirmation link sent (check console for MVP)" });
        }

        /// <summary>
        /// Сменить пароль
        /// </summary>
        [HttpPost("change-password")]
        [SwaggerOperation(Summary = "Сменить пароль")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await _mediator.Send(command);
            return Ok("Пароль успешно изменён");
        }

        /// <summary>
        /// Подтвердить email
        /// </summary>
        [HttpPost("confirm-email")]
        [SwaggerOperation(Summary = "Подтвердить email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand cmd, CancellationToken ct)
        {
            await _mediator.Send(cmd, ct);
            return Ok(new { message = "Email confirmed!" });
        }

        /// <summary>
        /// Запросить сброс пароля
        /// </summary>
        [HttpPost("request-password-reset")]
        [SwaggerOperation(Summary = "Запросить сброс пароля")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand cmd, CancellationToken ct)
        {
            await _mediator.Send(cmd, ct);
            return Ok(new { message = "Reset link sent (check console for MVP)" });
        }

        /// <summary>
        /// Сбросить пароль
        /// </summary>
        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Сбросить пароль")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand cmd, CancellationToken ct)
        {
            await _mediator.Send(cmd, ct);
            return Ok(new { message = "Password changed!" });
        }

        /// <summary>
        /// Получить свои claims и роли (пример защищённого метода)
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [SwaggerOperation(Summary = "Информация о пользователе из токена")]
        public IActionResult Me()
        {
            var claims = User.Claims.Select(x => new { x.Type, x.Value }).ToList();
            return Ok(new { Claims = claims });
        }
    }
}
