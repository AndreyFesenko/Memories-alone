using Microsoft.AspNetCore.Mvc;

namespace PublicViewService.Controllers
{
    [ApiController]
    [Route("api")]
    public class PublicViewController : ControllerBase
    {
        [HttpGet("profile/{userId:guid}")]
        public IActionResult GetProfile(Guid userId)
        {
            var dto = new
            {
                userId = userId,
                displayName = "Public User",
                bio = "This is a public profile stub.",
                avatarUrl = (string?)null
            };
            return Ok(dto);
        }

        [HttpGet("memory/{memoryId:guid}")]
        public IActionResult GetMemory(Guid memoryId)
        {
            var dto = new
            {
                id = memoryId,
                title = "Public memory stub",
                description = "Publicly visible memory description",
                mediaFiles = new[] { new { id = Guid.NewGuid(), fileName = "pic.jpg", mediaType = "image/jpeg", url = "https://example.com/pic.jpg" } }
            };
            return Ok(dto);
        }
    }
}
