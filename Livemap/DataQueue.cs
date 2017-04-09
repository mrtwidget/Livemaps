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
    public class DataQueue
    {

        public DataQueue()
        {
            RefreshLivemap();
        }

        /* REFRESH LIVEMAP DATA */
        public void RefreshLivemap()
        {
            if (Livemap.Instance.TimeSinceRefresh == null || ((DateTime.Now - Livemap.Instance.TimeSinceRefresh.Value).TotalSeconds > Livemap.Instance.Configuration.Instance.RefreshInterval))
            {
                Client[] clients = new Client[Provider.clients.Count];

                int clientCount = 0;
                foreach (SteamPlayer client in Provider.clients)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(client);

                    clients[clientCount] = new Client(
                        uPlayer.CharacterName,
                        uPlayer.Id,
                        uPlayer.SteamGroupID.ToString(),
                        uPlayer.SteamName,
                        uPlayer.SteamProfile.AvatarMedium.ToString(),
                        uPlayer.SteamProfile.Headline,
                        uPlayer.CSteamID.ToString(),
                        uPlayer.IsPro,
                        uPlayer.IsAdmin,
                        uPlayer.GodMode,
                        uPlayer.VanishMode,
                        uPlayer.IsInVehicle,
                        ReturnVehicleInfo(uPlayer, "instanceID"),
                        ReturnVehicleInfo(uPlayer, "id"),
                        ReturnVehicleInfo(uPlayer, "fuel"),
                        ReturnVehicleInfo(uPlayer, "health"),
                        ReturnVehicleInfo(uPlayer, "headlightsOn"),
                        ReturnVehicleInfo(uPlayer, "taillightsOn"),
                        ReturnVehicleInfo(uPlayer, "sirensOn"),
                        uPlayer.Position.ToString(),
                        uPlayer.Rotation,
                        uPlayer.Ping,
                        uPlayer.Dead,
                        uPlayer.Bleeding,
                        uPlayer.Broken,
                        uPlayer.Health,
                        uPlayer.Stamina,
                        uPlayer.Hunger,
                        uPlayer.Thirst,
                        uPlayer.Infection,
                        uPlayer.Experience,
                        uPlayer.Reputation,
                        IsHidden(uPlayer)

                        //uPlayer.GetSkillLevel(UnturnedSkill.Agriculture),
                        );

                    clientCount++;
                }

                /* WRITE XML FILE */
                using (XmlWriter writer = XmlWriter.Create(Livemap.Instance.Configuration.Instance.LivemapXML))
                {
                    writer.WriteStartDocument();

                    // <MAP>
                    writer.WriteStartElement(Provider.map.ToString());

                    // <STATS>
                    writer.WriteStartElement("Stats");
                    writer.WriteElementString("ServerName", Provider.serverName);
                    writer.WriteElementString("ClientVersion", Provider.APP_VERSION);
                    writer.WriteElementString("Map", Provider.map.ToString());
                    writer.WriteElementString("PlayersOnline", Provider.clients.Count.ToString());
                    writer.WriteElementString("MaxPlayers", Provider.maxPlayers.ToString());
                    writer.WriteElementString("PvP", Provider.isPvP.ToString());
                    writer.WriteElementString("CheatsEnabled", Provider.hasCheats.ToString());
                    writer.WriteElementString("HideAdmins", Provider.hideAdmins.ToString());
                    writer.WriteElementString("CycleTime", LightingManager.time.ToString());
                    writer.WriteElementString("CycleLength", LightingManager.cycle.ToString());
                    writer.WriteElementString("FullMoon", LightingManager.isFullMoon.ToString());
                    writer.WriteElementString("TotalUptime", Time.time.ToString());
                    writer.WriteElementString("PacketsReceived", Provider.packetsReceived.ToString());
                    writer.WriteElementString("PacketsSent", Provider.packetsSent.ToString());
                    writer.WriteElementString("ServerPort", Provider.port.ToString());
                    writer.WriteEndElement(); 
                    // </STATS>

                    // <PLAYERS>
                    writer.WriteStartElement("Players");

                    foreach (Client client in clients)
                    {
                        // <PLAYER>
                        writer.WriteStartElement("Player");
                        
                        writer.WriteElementString("CharacterName", client.CharacterName); // character name

                            // <STEAM>
                            writer.WriteStartElement("Steam");
                            writer.WriteElementString("Steam64ID", client.Steam64ID);
                            writer.WriteEndElement();
                            // </STEAM>

                            // <SETTINGS>
                            writer.WriteStartElement("Settings");
                            writer.WriteElementString("IsPro", client.IsPro.ToString());
                            writer.WriteElementString("IsAdmin", client.IsAdmin.ToString());
                            writer.WriteElementString("GodMode", client.GodMode.ToString());
                            writer.WriteElementString("VanishMode", client.VanishMode.ToString());
                            writer.WriteElementString("IsInVehicle", client.IsInVehicle.ToString());
                            writer.WriteElementString("IsHiddenLivemap", client.IsHiddenLivemap.ToString());
                            writer.WriteEndElement();
                            // </SETTINGS>

                            // <STATS>
                            writer.WriteStartElement("Stats");
                            writer.WriteElementString("Position", client.Position);
                            writer.WriteElementString("Rotation", client.Rotation.ToString());
                            writer.WriteElementString("Ping", client.Ping.ToString());
                            writer.WriteElementString("Dead", client.Dead.ToString());
                            writer.WriteElementString("Bleeding", client.Bleeding.ToString());
                            writer.WriteElementString("Broken", client.Broken.ToString());
                            writer.WriteElementString("Health", client.Health.ToString());
                            writer.WriteElementString("Stamina", client.Stamina.ToString());
                            writer.WriteElementString("Hunger", client.Hunger.ToString());
                            writer.WriteElementString("Thirst", client.Thirst.ToString());
                            writer.WriteElementString("Infection", client.Infection.ToString());
                            writer.WriteElementString("Experience", client.Experience.ToString());
                            writer.WriteEndElement();
                            // </STATS>

                            // <VEHICLE>
                            writer.WriteStartElement("Vehicle");
                            if (client.IsInVehicle)
                            {
                                writer.WriteElementString("VehicleInstanceID", client.CurrentVehicleInstanceID.ToString());
                                writer.WriteElementString("VehicleID", client.CurrentVehicleID.ToString());
                                writer.WriteElementString("VehicleFuel", client.CurrentVehicleFuel.ToString());
                                writer.WriteElementString("VehicleHealth", client.CurrentVehicleHealth.ToString());
                                writer.WriteElementString("HeadlightsON", client.headlightsOn.ToString());
                                writer.WriteElementString("TaillightsON", client.taillightsOn.ToString());
                                writer.WriteElementString("SirensON", client.sirensOn.ToString());
                            }
                            writer.WriteEndElement();
                            // </VEHICLE>

                        writer.WriteEndElement();
                        // </PLAYER>

                    }

                    writer.WriteEndElement();
                    // </PLAYERS>

                    writer.WriteEndElement();
                    // </MAP>


                    writer.WriteEndDocument();

                    // clean up
                    writer.Flush();
                    writer.Close();
                }

                // update lastLivemapRefresh time
                Livemap.Instance.TimeSinceRefresh = DateTime.Now;
            }
        }
        
        /* RETURN VEHICLE INFORMATION */
        public string ReturnVehicleInfo(UnturnedPlayer client, string setting)
        {
            if (client.IsInVehicle)
            {
                switch (setting)
                {
                    case "instanceID":
                        return client.CurrentVehicle.instanceID.ToString();

                    case "id":
                        return client.CurrentVehicle.id.ToString();

                    case "fuel":
                        return client.CurrentVehicle.fuel.ToString();

                    case "health":
                        return client.CurrentVehicle.health.ToString();

                    case "headlightsOn":
                        return client.CurrentVehicle.headlightsOn.ToString();

                    case "taillightsOn":
                        return client.CurrentVehicle.taillightsOn.ToString();

                    case "sirensOn":
                        return client.CurrentVehicle.sirensOn.ToString();

                    default:
                        return "False";
                }
            }
            else
            {
                return "False";
            }
        }

        /* IS CLIENT HIDDEN */
        public static bool IsHidden(UnturnedPlayer client)
        {
            if (Livemap.HiddenPlayers[client.CSteamID.ToString()] == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /* RETRIEVE CLIENT DATA */
        public class Client
        {
            string _CharacterName;
            string _Id;
            string _SteamGroupID;
            string _SteamName;
            string _SteamAvatarMedium;
            string _SteamProfileHeadline;
            string _Steam64ID;
            //string _IP;
            bool _IsPro;
            bool _IsAdmin;
            bool _GodMode;
            bool _VanishMode;
            bool _IsInVehicle;
            string _CurrentVehicleInstanceID;
            string _CurrentVehicleID;
            string _CurrentVehicleFuel;
            string _CurrentVehicleHealth;
            string _headlightsOn;
            string _taillightsOn;
            string _sirensOn;
            string _Position;
            float _Rotation;
            float _Ping;
            bool _Dead;
            bool _Bleeding;
            bool _Broken;
            byte _Health;
            byte _Stamina;
            byte _Hunger;
            byte _Thirst;
            byte _Infection;
            uint _Experience;
            int _Reputation;
            bool _IsHiddenLivemap;

            public Client(string CharacterName, string Id, string SteamGroupID, string SteamName, string SteamAvatarMedium, string SteamProfileHeadline, string Steam64ID, /* string IP, */ bool IsPro, bool IsAdmin, bool GodMode, bool VanishMode, bool IsInVehicle, string CurrentVehicleInstanceID, string CurrentVehicleID, string CurrentVehicleFuel, string CurrentVehicleHealth, string headlightsOn, string taillightsOn, string sirensOn, string Position, float Rotation, float Ping, bool Dead, bool Bleeding, bool Broken, byte Health, byte Stamina, byte Hunger, byte Thirst, byte Infection, uint Experience, int Reputation, bool IsHiddenLivemap)
            {
                this._CharacterName = CharacterName;
                this._Id = Id;
                this._SteamGroupID = SteamGroupID;
                this._SteamName = SteamName;
                this._SteamAvatarMedium = SteamAvatarMedium;
                this._SteamProfileHeadline = SteamProfileHeadline;
                this._Steam64ID = Steam64ID;
                //this._IP = IP;
                this._IsPro = IsPro;
                this._IsAdmin = IsAdmin;
                this._GodMode = GodMode;
                this._VanishMode = VanishMode;
                this._IsInVehicle = IsInVehicle;
                this._CurrentVehicleInstanceID = CurrentVehicleInstanceID;
                this._CurrentVehicleID = CurrentVehicleID;
                this._CurrentVehicleFuel = CurrentVehicleFuel;
                this._CurrentVehicleHealth = CurrentVehicleHealth;
                this._headlightsOn = headlightsOn;
                this._taillightsOn = taillightsOn;
                this._sirensOn = sirensOn;
                this._Position = Position;
                this._Rotation = Rotation;
                this._Ping = Ping;
                this._Dead = Dead;
                this._Bleeding = Bleeding;
                this._Broken = Broken;
                this._Health = Health;
                this._Stamina = Stamina;
                this._Hunger = Hunger;
                this._Thirst = Thirst;
                this._Infection = Infection;
                this._Experience = Experience;
                this._Reputation = Reputation;
                this._IsHiddenLivemap = IsHiddenLivemap;
            }

            public string CharacterName { get { return _CharacterName; } }
            public string Id { get { return _Id; } }
            public string SteamGroupID { get { return _SteamGroupID; } }
            public string SteamName { get { return _SteamName; } }
            public string SteamAvatarMedium { get { return _SteamAvatarMedium; } }
            public string SteamProfileHeadline { get { return _SteamProfileHeadline; } }
            public string Steam64ID { get { return _Steam64ID; } }
            //public string IP { get { return _IP; } }
            public bool IsPro { get { return _IsPro; } }
            public bool IsAdmin { get { return _IsAdmin; } }
            public bool GodMode { get { return _GodMode; } }
            public bool VanishMode { get { return _VanishMode; } }
            public bool IsInVehicle { get { return _IsInVehicle; } }
            public string CurrentVehicleInstanceID { get { return _CurrentVehicleInstanceID; } }
            public string CurrentVehicleID { get { return _CurrentVehicleID; } }
            public string CurrentVehicleFuel { get { return _CurrentVehicleFuel; } }
            public string CurrentVehicleHealth { get { return _CurrentVehicleHealth; } }
            public string headlightsOn { get { return _headlightsOn; } }
            public string taillightsOn { get { return _taillightsOn; } }
            public string sirensOn { get { return _sirensOn; } }
            public string Position { get { return _Position; } }
            public float Rotation { get { return _Rotation; } }
            public float Ping { get { return _Ping; } }
            public bool Dead { get { return _Dead; } }
            public bool Bleeding { get { return _Bleeding; } }
            public bool Broken { get { return _Broken; } }
            public byte Health { get { return _Health; } }
            public byte Stamina { get { return _Stamina; } }
            public byte Hunger { get { return _Hunger; } }
            public byte Thirst { get { return _Thirst; } }
            public byte Infection { get { return _Infection; } }
            public uint Experience { get { return _Experience; } }
            public int Reputation { get { return _Reputation; } }
            public bool IsHiddenLivemap { get { return _IsHiddenLivemap; } }

        }
    }
}
