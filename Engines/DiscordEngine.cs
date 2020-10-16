using LSSDReportHelper.Models;
using LSSDReportHelper.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LSSDReportHelper.Engines
{
    public class DiscordEngine
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ConfigService _config = new ConfigService();

        public DiscordEngine()
        {
            var token = _config.GetDiscordToken();
            if (token != null && !token.Equals(""))
            {
                _httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), token);
            }
        }

        public async Task<bool> SignIn(string email, string password)
        {
            var body = new DiscordLoginModel
            {
                email = email,
                password = password
            };

            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://discord.com/api/v8/auth/login", httpContent);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var textResponse = await response.Content.ReadAsStringAsync();
                var rss = JObject.Parse(textResponse);
                _config.SaveDiscordToken((string)rss["token"]);
            }

            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task SendMessage(long server, string message)
        {
            var body = new DiscordMessage
            {
                content = message
            };

            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"https://discord.com/api/v8/channels/{server}/messages"),
                Method = HttpMethod.Post,
                Content = httpContent
            };
            
            var httpResponse = await _httpClient.SendAsync(request);
            //await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Вы не прошли авторизацию или ваш пароль поменялся..");
            }
        }

        public async Task SendMessageWithFile(long server, string message, string path)
        {
            var attach = File.ReadAllBytes(path);
            var form = new MultipartFormDataContent
            {
                {new StringContent(message), "content"},
                {new ByteArrayContent(attach), "file", path}
            };

            var httpResponse = await _httpClient.PostAsync($"https://discord.com/api/v8/channels/{server}/messages", form);
            //await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Вы не прошли авторизацию или ваш пароль поменялся..");
            }
        }
    }
}
