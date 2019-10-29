using System;
using Steamworks;

namespace NEXIS.Livemap
{
    public class Server
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ConnectionAddress { get; set; }
        public string Map { get; set; }
        public byte MaxPlayers { get; set; }
        public int PlayersOnline { get; set; }
        public DateTime LastRefresh { get; set; }
        public uint Time { get; set; }
        public uint Cycle { get; set; }
        public bool FullMoon { get; set; }
        public string Version { get; set; }
        public bool PVP { get; set; }
        public bool Gold { get; set; }
        public bool HasCheats { get; set; }
        public bool HideAdmins { get; set; }
        public string Mode { get; set; }
        public int RefreshInterval { get; set; }
    }
}
