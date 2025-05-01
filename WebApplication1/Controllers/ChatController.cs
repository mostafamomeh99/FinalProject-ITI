using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RepositoryFactory;
using System.Data;
using WebApplication1.Dtos.ChatBot;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {

        private readonly IUnitOfWork unitOFWork;

        public ChatController(IUnitOfWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }

        [HttpPost]
        public async Task<IActionResult> GetResponse([FromBody] List<ChatBotSendDto> usermessages)
        {
            List<string> messages = new List<string>() { };
            List<string> roles = new List<string>() { };
            foreach (var item in usermessages)
            {
                if (item.Message.Length >= 14000) {
                    // check message length
                    return BadRequest(new { Role = "error", Message = "message is too long" });
                }
                roles.Add(item.Role);
                messages.Add(item.Message);
            }

                //llama2 - 70b - 4096 4,096 tokens - around 12,000–16,000 characters,
                 unitOFWork.GrokService.VerifiyMessageSize(messages, roles);

            var result = await unitOFWork.GrokService.SendMessage(messages, roles);
            if(result == null)
            {
                return BadRequest(new { Role = "error", Message = "something went wrong please try again" });
            }

            if (result.ContainsKey("error"))
            {
                var errorMessage = result["error"]?.ToString();
                return BadRequest(new { Role = "error", Message = "something went wrong , please try again" });
            }

            string role = result["choices"][0]["message"]["role"]?.ToString();
            string message = result["choices"][0]["message"]["content"]?.ToString();

            return Ok(new { Role = role, Message = message });

        }
    }
}
