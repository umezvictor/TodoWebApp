using Web.Client.DTOs;

namespace Web.Client.Responses
{
    public class GetTodosResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public Errors? Errors { get; set; }
        public List<Todo>? Data { get; set; }
    }


    public class Errors
    {
    }

}
