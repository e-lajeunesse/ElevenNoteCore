using System.ComponentModel.DataAnnotations;

namespace ElevenNote.Models.Token
{
    public class TokenRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}