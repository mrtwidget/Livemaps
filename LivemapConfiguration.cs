using Rocket.API;

namespace NEXIS.Livemap
{
    public class LivemapConfiguration : IRocketPluginConfiguration
    {
        public bool LivemapEnabled;
        public bool LivemapStatusEnabled;
        public int LivemapRefreshInterval;
        public bool WorldChatEnabled;
        public bool PlayerHideEnabled;
        public bool PlayerHideCooldownEnabled;
        public int PlayerHideDuration;
        public int PlayerHideCooldownDuration;
        public string PlayerDefaultSteamAvatar;

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
            LivemapEnabled = true;
            LivemapStatusEnabled = true;
            LivemapRefreshInterval = 10;
            WorldChatEnabled = true;
            PlayerHideEnabled = true;
            PlayerHideCooldownEnabled = true;
            PlayerHideDuration = 300;
            PlayerHideCooldownDuration = 600;
            PlayerDefaultSteamAvatar = "images/avatars/unknown.png";

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
