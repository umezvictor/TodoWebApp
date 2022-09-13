using Web.Client.DTOs;

namespace Web.Client.Responses
{
   

    public class TodoResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public Errors? Errors { get; set; }
        public Todo? Data { get; set; }
    }


}
