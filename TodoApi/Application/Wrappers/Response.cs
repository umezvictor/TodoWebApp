using System.Collections.Generic;

namespace Application.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, string message)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }

        public Response(T data)
        {
            Succeeded = true;            
            Data = data;
        }
        public Response(string message)
        {
            Succeeded = false;
            Message = message;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        //public List<string> Errors { get; set; } = new List<string>();
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
       // public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
        public T? Data { get; set; }
    }
}