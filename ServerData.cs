using System;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using SDG.Unturned;
using UnityEngine;
using System.Xml;
using Rocket.Unturned;
using Steamworks;

namespace NEXIS.Livemap
{
    public class ServerData
    {
        string _serverID;
        string _serverName;
        string _appVersion;
        string _map;
        int _clients;
        byte _maxPlayers;
        bool _isPvP;
        bool _isGold;
        bool _hasCheats;
        bool _hideAdmins;
        uint _cycleTime;
        uint _cycleLength;
        bool _isFullMoon;
        float _time;
        uint _packetsReceived;
        uint _packetsSent;
        ushort _port;
        EGameMode _mode;

        public ServerData(string serverID, string serverName, string appVersion, string map, int clients, byte maxPlayers,
            bool isPvP, bool isGold, bool hasCheats, bool hideAdmins, uint cycleTime, uint cycleLength, bool isFullMoon,
            float time, uint packetsReceived, uint packetsSent, ushort port, EGameMode mode)
        {
            this._serverID = serverID;
            this._serverName = serverName;
            this._appVersion = appVersion;
            this._map = map;
            this._clients = clients;
            this._maxPlayers = maxPlayers;
            this._isPvP = isPvP;
            this._isGold = isGold;
            this._hasCheats = hasCheats;
            this._hideAdmins = hideAdmins;
            this._cycleTime = cycleTime;
            this._cycleLength = cycleLength;
            this._isFullMoon = isFullMoon;
            this._time = time;
            this._packetsReceived = packetsReceived;
            this._packetsSent = packetsSent;
            this._port = port;
            this._mode = mode;
        }

        public string serverID { get { return _serverID; } }
        public string serverName { get { return _serverName; } }
        public string appVersion { get { return _appVersion; } }
        public string map { get { return _map; } }
        public int clients { get { return _clients; } }
        public byte maxPlayers { get { return _maxPlayers; } }
        public bool isPvP { get { return _isPvP; } }
        public bool isGold { get { return _isGold; } }
        public bool hasCheats { get { return _hasCheats; } }
        public bool hideAdmins { get { return _hideAdmins; } }
        public uint cycleTime { get { return _cycleTime; } }
        public uint cycleLength { get { return _cycleLength; } }
        public bool isFullMoon { get { return _isFullMoon; } }
        public float time { get { return _time; } }
        public uint packetsReceived { get { return _packetsReceived; } }
        public uint packetsSent { get { return _packetsSent; } }
        public ushort port { get { return _port; } }
        public EGameMode mode { get { return _mode; } }
    }    
}
