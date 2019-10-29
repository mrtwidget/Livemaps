using Rocket.API;

namespace NEXIS.Livemap
{
    public class LivemapConfiguration : IRocketPluginConfiguration
    {
        public bool LivemapDebug;
        public bool LivemapEnabled;
        public bool MySQLEnabled;
        public int LivemapRefreshInterval;
        public bool WorldChatEnabled;
        public bool PlayerHidingEnabled;
        public bool ShowCommandsInChat;
        public string PlayerDefaultSteamAvatar;
        public string ConnectionAddress;
        public string WebsiteURI;

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
            LivemapDebug = false;
            LivemapEnabled = true;
            MySQLEnabled = false;
            LivemapRefreshInterval = 10;
            WorldChatEnabled = true;
            PlayerHidingEnabled = true;
            ShowCommandsInChat = false;
            PlayerDefaultSteamAvatar = "images/avatars/unknown.png";
            ConnectionAddress = "0.0.0.0:27016";
            WebsiteURI = "http://yourwebsite.com/livemap/api/livemap.api.php";

            // Database Settings
            DatabaseHost = "127.0.0.1";
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
