namespace LSSDReportHelper.Models
{
    public class DiscordLoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public bool undelete { get; set; } = false;
        public object captcha_key { get; set; } = null;
        public object gift_code_sku_id { get; set; } = null;
        public object login_source { get; set; } = null;
    }
}
