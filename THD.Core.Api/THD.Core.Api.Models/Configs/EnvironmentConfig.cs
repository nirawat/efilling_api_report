namespace THD.Core.Api.Models.Config
{
    public interface IEnvironmentConfig
    {
        string Server { get; set; }
        string BaseRoute { get; set; }
        string PathArchive { get; set; }
        string PathDocument { get; set; }
        string PathReport { get; set; }
    }

    public class EnvironmentConfig : IEnvironmentConfig
    {
        public string Server { get; set; }
        public string BaseRoute { get; set; }
        public string PathArchive { get; set; }
        public string PathDocument { get; set; }
        public string PathReport { get; set; }
    }

}
