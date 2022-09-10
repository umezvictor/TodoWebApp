namespace Web.Client.Responses
{
   
    public class AuthResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string Payload { get; set; } = string.Empty;

    }
}
