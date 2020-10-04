using LSSDReportHelper.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LSSDReportHelper.Engines
{
    public class DiscordEngine
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task SignIn(string email, string password)
        {
            var body = new DiscordLoginModel
            {
                email = email,
                password = password
            };

            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync("https://discord.com/api/v8/auth/login", httpContent);
            //await httpResponse.Content.ReadAsStringAsync();
        }
    }
}
