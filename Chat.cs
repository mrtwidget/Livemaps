using System;
using Steamworks;

namespace NEXIS.Livemap
{
    public class Chat
    {
        public CSteamID SteamID { get; set; }
        public string CharacterName { get; set; }
        public string Avatar { get; set; }
        public bool IsAdmin { get; set; }
        public string Message { get; set; }
    }
}
