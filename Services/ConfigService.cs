using System.IO;
using LSSDReportHelper.Models;
using Newtonsoft.Json;

namespace LSSDReportHelper.Services
{
    public class ConfigService
    {
        public ConfigModel GetConfig()
        {
            var config = File.ReadAllText("config.json");
            return JsonConvert.DeserializeObject<ConfigModel>(config);
        }

        public void SaveConfig(ConfigModel config)
        {
            var text = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText("config.json", text);
        }

        public string GetDiscordToken() => GetConfig().DiscordToken;

        public void SaveDiscordToken(string token)
        {
            var config = GetConfig();
            config.DiscordToken = token;
            SaveConfig(config);
        }

        public void RemoveDiscordToken()
        {
            var config = GetConfig();
            config.DiscordToken = null;
            SaveConfig(config);
        } 
    }
}
