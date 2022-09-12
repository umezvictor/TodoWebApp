using System.Text;

namespace Web.Client.Utils
{
    public static class ApiClient
    {
        public static string GetAsync(string uri, string apiToken)
        {
            string resp = "";

            try
            {

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");
                var reponse = client.GetAsync(uri);
                if (reponse.Result.IsSuccessStatusCode)
                {
                    resp = reponse.Result.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    resp = reponse.Result.Content.ReadAsStringAsync().Result;
                }
                return resp;
            }
            catch (Exception ex)
            {
                return "ERROR:: " + ex.ToString();
            }
        }


        public static string PostAsync(string uri, string data, string apiToken)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");
                string res = "";
               

                var response = client.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));

                var apiResponse = response.Result;

                if (apiResponse.IsSuccessStatusCode)
                {
                    res = apiResponse.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    res = apiResponse.Content.ReadAsStringAsync().Result;

                }
                return res;
            }
            catch (Exception ex)
            {

                return "ERROR:: " + ex.ToString();
            }

        }



        public static string PutAsync(string uri, string data, string apiToken)
        {
            try
            {
                HttpClient client = new HttpClient();
                string res = "";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

                var response = client.PutAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));

                var apiResponse = response.Result;

                if (apiResponse.IsSuccessStatusCode)
                {
                    res = apiResponse.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    res = apiResponse.Content.ReadAsStringAsync().Result;

                }
                return res;
            }
            catch (Exception ex)
            {

                return "ERROR:: " + ex.ToString();
            }

        }


        public static string DeleteAsync(string uri, string apiToken)
        {
            try
            {
                HttpClient client = new HttpClient();
                string res = "";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

                var response = client.DeleteAsync(uri);

                var apiResponse = response.Result;

                if (apiResponse.IsSuccessStatusCode)
                {
                    res = apiResponse.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    res = apiResponse.Content.ReadAsStringAsync().Result;

                }
                return res;
            }
            catch (Exception ex)
            {

                return "ERROR:: " + ex.ToString();
            }

        }
    } 

}
