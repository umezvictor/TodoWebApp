using System.ComponentModel.DataAnnotations;

namespace Web.Client.DTOs
{

    public class AddTodoDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

    }
}
