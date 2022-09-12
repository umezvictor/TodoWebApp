﻿namespace Web.Client.Responses
{
   
    public class ApiTokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string token_type { get; set; } = string.Empty;
        public string scope { get; set; } = string.Empty;
    }


}
