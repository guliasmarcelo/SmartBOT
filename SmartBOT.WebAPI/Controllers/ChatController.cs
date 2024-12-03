
using Microsoft.AspNetCore.Mvc;
using SmartBOT.WebAPI.Core;

namespace SmartBOT.WebAPI.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly TeslaHelpDeskIntegrationService _integrationService;

    public ChatController(TeslaHelpDeskIntegrationService integrationService)
    {
        _integrationService = integrationService;
    }

    [HttpPost("{helpdeskId}")]
    public async Task<IActionResult> SendMessage(string helpdeskId, [FromBody] UserMessageRequest request)
    {
        try
        {
            var response = await _integrationService.HandleUserQueryAsync(helpdeskId, request.UserMessage);
            return Ok(new { Response = response });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

public class UserMessageRequest
{
    public string UserMessage { get; set; }
}