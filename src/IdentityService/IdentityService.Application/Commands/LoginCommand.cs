using System.ComponentModel.DataAnnotations;
using MediatR;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Commands
{
    /// <summary>
    /// Модель запроса на вход (login)
    /// </summary>
    public class LoginCommand : IRequest<LoginResponse>
    {
        /// <example>user@example.com</example>
        [Required]
        public string Email { get; set; } = default!;

        /// <example>P@ssword123</example>
        [Required]
        public string Password { get; set; } = default!;
    }
}