namespace THD.Core.Api.Models.Config
{

    public interface IEmailConfig
    {
        string Host { get; set; }
        string Port { get; set; }
        string User { get; set; }
        string Pass { get; set; }
    }

    public class EmailConfig : IEmailConfig
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
    }
}
