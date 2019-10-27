using System;
using Steamworks;

namespace NEXIS.Livemap
{
    public class Nodes
    {
        public CSteamID SteamID { get; set; }
        public string CharacterName { get; set; }
        public string Avatar { get; set; }
        public string Position { get; set; }
        public uint Experience { get; set; }
        public int Reputation { get; set; }
        public bool Dead { get; set; }
        public byte Health { get; set; }
        public byte Hunger { get; set; }
        public byte Infection { get; set; }
        public byte Stamina { get; set; }
        public byte Thirst { get; set; }
        public bool Bleeding { get; set; }
        public bool Broken { get; set; }
        public bool GodMode { get; set; }
        public bool VanishMode { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsInVehicle { get; set; }
        public bool IsPro { get; set; }
        public float Ping { get; set; }
        public string IP { get; set; }
        //public string Skin { get; set; }
        public string Face { get; set; }
        public bool Hidden { get; set; }
        public DateTime ConnectionTime { get; set; }
    }
}
