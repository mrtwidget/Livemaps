using Rocket.API;

namespace NEXIS.Livemap
{
    public class LivemapConfiguration : IRocketPluginConfiguration
    {
        public int RefreshInterval;
        public bool PlayersCanHide;
        public bool EnableHideCooldown;
        public int PlayerHideDuration;
        public int HideCooldownDuration;

        public string DatabaseHost;
        public string DatabaseUser;
        public string DatabasePass;
        public string DatabaseName;
        public int DatabasePort;
        public string DatabaseTableLivemapServers;
        public string DatabaseTableLivemapData;

        public void LoadDefaults()
        {
            // General Settings
            RefreshInterval = 15;
            PlayersCanHide = true;
            EnableHideCooldown = false;
            PlayerHideDuration = 300;
            HideCooldownDuration = 600;

            // Database Settings
            DatabaseHost = "localhost";
            DatabaseUser = "unturned";
            DatabasePass = "password";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            DatabaseTableLivemapServers = "livemap_servers";
            DatabaseTableLivemapData = "livemap_data";            
        }
    }
}
