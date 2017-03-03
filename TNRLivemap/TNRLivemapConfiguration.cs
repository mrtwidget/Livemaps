using Rocket.API;

namespace NEXIS.TNRLivemap
{
    public class TNRLivemapConfiguration : IRocketPluginConfiguration
    {
        public int RefreshInterval;
        public string LivemapXML;
        public bool PlayersCanHide;
        public int HideDuration;
        public int CooldownDuration;

        public string DatabaseHost;
        public string DatabaseUser;
        public string DatabasePass;
        public string DatabaseName;
        public int DatabasePort;
        public string DatabaseTable;

        public void LoadDefaults()
        {
            // General Settings
            RefreshInterval = 15;
            LivemapXML = @"Livemap.xml";
            PlayersCanHide = true;
            HideDuration = 300;
            CooldownDuration = 600;

            // Database Credentials
            DatabaseHost = "localhost";
            DatabaseUser = "unturned";
            DatabasePass = "password";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            DatabaseTable = "livemap";
        }
    }
}
