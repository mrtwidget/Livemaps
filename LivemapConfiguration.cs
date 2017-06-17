using Rocket.API;

namespace NEXIS.Livemap
{
    public class LivemapConfiguration : IRocketPluginConfiguration
    {
        public int RefreshInterval;
        public bool EnableWorldChat;
        public bool EnableHiding;
        public bool EnableHideCooldown;
        public int PlayerHideDuration;
        public int HideCooldownDuration;
        public string DefaultSteamAvatar;

        public string DatabaseHost;
        public string DatabaseUser;
        public string DatabasePass;
        public string DatabaseName;
        public int DatabasePort;
        public string DatabaseTableLivemapServer;
        public string DatabaseTableLivemapData;
        public string DatabaseTableLivemapChat;

        public void LoadDefaults()
        {
            // General Settings
            RefreshInterval = 15;
            EnableWorldChat = true;
            EnableHiding = true;
            EnableHideCooldown = false;
            PlayerHideDuration = 300;
            HideCooldownDuration = 600;
            DefaultSteamAvatar = "images/avatars/unknown.png";

            // Database Settings
            DatabaseHost = "localhost";
            DatabaseUser = "unturned";
            DatabasePass = "password";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            DatabaseTableLivemapServer = "livemap_server";
            DatabaseTableLivemapData = "livemap_data";
            DatabaseTableLivemapChat = "livemap_chat";
        }
    }
}
