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
            ConnectionAddress = "nexisrealms.net:27016";
            WebsiteURI = "http://nexisrealms.com/livemap/api/livemap.api.php";
        }
    }
}
