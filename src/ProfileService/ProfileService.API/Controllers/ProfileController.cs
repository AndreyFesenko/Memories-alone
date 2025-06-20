using Microsoft.AspNetCore.Mvc;
using ProfileService.Application.Commands;
using ProfileService.Application.DTOs;
using MediatR;


namespace ProfileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить профиль пользователя
        /// </summary>
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserProfileDto>> Get(Guid userId)
        {
            var result = await _mediator.Send(new GetProfileQuery { UserId = userId });
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Создать или обновить профиль
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserProfileDto>> Upsert([FromBody] UpsertProfileCommand command)
        {
            var profile = await _mediator.Send(command);
            return Ok(profile);
        }

        /// <summary>
        /// Подтвердить смерть пользователя (MVP)
        /// </summary>
        [HttpPost("{userId:guid}/confirm-death")]
        public async Task<ActionResult> ConfirmDeath(Guid userId)
        {
            await _mediator.Send(new ConfirmDeathCommand { UserId = userId });
            return Ok(new { message = "Death confirmed" });
        }
    }
}
