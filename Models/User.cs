using System.ComponentModel.DataAnnotations;

namespace MyTodoList.Models // DİKKAT: Artık MyTodoList oldu
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string SecurityQuestion { get; set; } = string.Empty;
        public string SecurityAnswer { get; set; } = string.Empty;
    }
}