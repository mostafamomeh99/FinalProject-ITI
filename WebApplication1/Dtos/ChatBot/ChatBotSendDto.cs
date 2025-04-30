using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos.ChatBot
{
    public class ChatBotSendDto
    {
        [Required(ErrorMessage ="data is requried")]
        public string Role { get; set; }
        [Required(ErrorMessage = "data is requried")]
        public string Message { get; set; }

    }
}
