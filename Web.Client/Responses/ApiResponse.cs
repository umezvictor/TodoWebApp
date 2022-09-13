namespace Web.Client.Responses
{
   
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } 
        public T? Payload { get; set; }
    }
}
