using Newtonsoft.Json.Linq;

namespace PopulationAPI.Helper
{
    public static class HttpHelper
    {
        public static string Get(string url)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync(url).Result;
            return  response.Content.ReadAsStringAsync().Result;
        }
    }
}
