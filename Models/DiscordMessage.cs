namespace LSSDReportHelper.Models
{
    public class DiscordMessage
    {
        public string content { get; set; }
        public bool tts { get; set; } = false;
        public DiscordMessageEmbed embed { get; set; }
    }
}
