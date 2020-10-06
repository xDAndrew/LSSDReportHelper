using LSSDReportHelper.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSSDReportHelper.Engines
{
    public class DiscordEngine
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public DiscordEngine()
        {
            _httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), "MjgwMzE1NzkxMjM5MjgyNjg5.X3w6JA.vNArO3xEiuPo6BVgh4OaSELR9lw");
        }

        public async Task SignIn(string email, string password)
        {
            var body = new DiscordLoginModel
            {
                email = email,
                password = password
            };

            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("https://discord.com/api/v8/auth/login", httpContent);
            //await httpResponse.Content.ReadAsStringAsync();
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
            await httpResponse.Content.ReadAsStringAsync();
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
            var res = await httpResponse.Content.ReadAsStringAsync();
            //MessageBox.Show(res);
        }
    }
}
