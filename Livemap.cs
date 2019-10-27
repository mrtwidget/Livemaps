using Logger = Rocket.Core.Logging.Logger;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Net;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.API;
using UnityEngine;

namespace NEXIS.Livemap
{
    public class Livemap : RocketPlugin<LivemapConfiguration>
    {
        #region Fields
        
        public static Livemap Instance;

        public DateTime? LastRefresh;
        public Dictionary<CSteamID, Nodes> Nodes;
        public Dictionary<int, Chat> Chat;
        public int LastChatID;

        #endregion

        #region Overrides

        protected override void Load()
        {
            Instance = this;
            
            Nodes = new Dictionary<CSteamID, Nodes>();
            Chat = new Dictionary<int, Chat>();
            LastChatID = 0;

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead += Events_OnPlayerDead;
            UnturnedPlayerEvents.OnPlayerRevive += Events_OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerChatted += Events_OnPlayerChatted;

            // load connected players if loaded on-the-fly
            if (Provider.clients.Count > 0)
            {
                Logger.Log(Provider.clients.Count + " are currently connected. Loading...", ConsoleColor.Yellow);

                new Thread(() =>
                {
                    // loop through all players
                    for (int i = 0; i < Provider.clients.Count; i++)
                    {
                        SteamPlayer plr = Provider.clients[i];

                        if (plr == null) continue;

                        UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(plr);

                        Events_OnPlayerConnected(player);
                    }
                }).Start();
            }

            Logger.Log("Livemaps successfully loaded!", ConsoleColor.Green);
        }

        protected override void Unload()
        {            
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead -= Events_OnPlayerDead;
            UnturnedPlayerEvents.OnPlayerRevive -= Events_OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerChatted -= Events_OnPlayerChatted;

            Logger.Log("Livemaps successfully unloaded!", ConsoleColor.Green);
        }
        
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"livemap_hidden_disabled", "Livemap hiding is currently disabled! :("},
            {"livemap_hidden_true", "Your location is hidden on the Livemap!"},
            {"livemap_hidden_false", "Your location is now visible to anyone on the Livemap!"}
        };
        
        #endregion
        
        #region Events

        public void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            new Thread(() =>
            {
                // get player avatar from steam
                ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
                XmlDocument xml = new XmlDocument();
                xml.Load("https://steamcommunity.com/profiles/" + player.CSteamID.ToString() + "?xml=1");
                XmlNode node = xml.SelectSingleNode(String.Format("//*[local-name()='{0}']", "avatarMedium"));

                // create new player node
                Nodes plr = new Nodes { };
                plr.Avatar = node.InnerText;
                plr.CharacterName = player.CharacterName;
                plr.ConnectionTime = DateTime.Now;
                plr.Dead = player.Dead;
                plr.Experience = player.Experience;
                plr.GodMode = player.GodMode;
                plr.Health = player.Health;
                plr.Hunger = player.Hunger;
                plr.Infection = player.Infection;
                plr.IP = player.IP;
                plr.IsAdmin = player.IsAdmin;
                plr.IsInVehicle = player.IsInVehicle;
                plr.IsPro = player.IsPro;
                plr.Ping = player.Ping;
                plr.Position = player.Position.ToString();
                plr.Reputation = player.Reputation;
                plr.Stamina = player.Stamina;
                plr.SteamID = player.CSteamID;
                plr.Thirst = player.Thirst;
                plr.VanishMode = player.VanishMode;
                //plr.Skin = ColorTypeConverter.ToRGBHex(player.Player.clothing.skin);
                plr.Face = player.Player.clothing.face.ToString();
                plr.Hidden = false;

                Nodes.Add(player.CSteamID, plr);
            }).Start();
        }

        public void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            // remove player from player nodes
            if (Nodes.ContainsKey(player.CSteamID))
                Nodes.Remove(player.CSteamID);
        }

        public void Events_OnPlayerDead(UnturnedPlayer player, Vector3 position)
        {
            // update dead status
            Nodes[player.CSteamID].Dead = player.Dead;
        }

        public void Events_OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
            // update dead status
            Nodes[player.CSteamID].Dead = player.Dead;
        }

        public void Events_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            if (Configuration.Instance.WorldChatEnabled && chatMode == EChatMode.GLOBAL)
            {
                if (message.StartsWith("/") && !Configuration.Instance.ShowCommandsInChat)
                    return;

                // create new chat message
                Chat msg = new Chat {};
                msg.SteamID = player.CSteamID;
                msg.CharacterName = player.CharacterName;
                msg.Avatar = Nodes[player.CSteamID].Avatar;
                msg.IsAdmin = player.IsAdmin;
                msg.Message = message;

                Chat.Add(LastChatID++, msg);
            }
        }

        #endregion

        public void FixedUpdate()
        {
            if (Instance.State != PluginState.Loaded) return;

            if (Instance.LastRefresh == null || (DateTime.Now - Instance.LastRefresh.Value).TotalSeconds > Instance.Configuration.Instance.LivemapRefreshInterval)
            {
                if (Configuration.Instance.MySQLEnabled)
                {
                    /* Update MySQL */
                    /* TODO */
                }
                else
                {
                    /* Update JSON */
                    UpdatePlayers();
                    SendData();
                }
                    
            }
        }

        public void UpdatePlayers()
        {
            new Thread(() =>
            {            
                // loop through all players
                for (int i = 0; i < Provider.clients.Count; i++)
                {
                    SteamPlayer plr = Provider.clients[i];

                    if (plr == null) continue;

                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(plr);

                    // update player data
                    Nodes[player.CSteamID].Position = player.Position.ToString();
                    Nodes[player.CSteamID].Reputation = player.Reputation;
                    Nodes[player.CSteamID].Experience = player.Experience;
                    Nodes[player.CSteamID].Face = player.Player.clothing.face.ToString();
                    //Nodes[player.CSteamID].Skin = ColorTypeConverter.ToRGBHex(player.Player.clothing.skin);
                    Nodes[player.CSteamID].VanishMode = player.VanishMode;
                    Nodes[player.CSteamID].GodMode = player.GodMode;
                    Nodes[player.CSteamID].IsAdmin = player.IsAdmin;
                    Nodes[player.CSteamID].IsInVehicle = player.IsInVehicle;
                    Nodes[player.CSteamID].Dead = player.Dead;
                    Nodes[player.CSteamID].Health = player.Health;
                    Nodes[player.CSteamID].Hunger = player.Hunger;
                    Nodes[player.CSteamID].Thirst = player.Thirst;
                    Nodes[player.CSteamID].Infection = player.Infection;
                    Nodes[player.CSteamID].Stamina = player.Stamina;
                    Nodes[player.CSteamID].Bleeding = player.Bleeding;
                    Nodes[player.CSteamID].Broken = player.Broken;
                }
            }).Start();

            // update refresh interval
            LastRefresh = DateTime.Now;
        }

        public void SendData()
        {
            new Thread(() =>
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

                try { 
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(Instance.Configuration.Instance.WebsiteURI);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{";

                            json += returnJSONServer();

                            json += returnJSONPlayers();

                            if (Configuration.Instance.WorldChatEnabled)
                                json += returnJSONChat();
   
                        json += "}";


                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (Configuration.Instance.LivemapDebug)
                        Console.WriteLine("Livemap Update Response: {0}", httpResponse.StatusDescription);

                }
                catch (WebException e)
                {
                    Console.WriteLine("Livemap WebException Raised. The following error occurred : {0} :: {1}", e.Status, e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The following Livemap Exception was raised : {0}", e.Message);
                }

            }).Start();
        }

        public string returnJSONServer()
        {
            return "\"Server\": {" +
                    "\"ID\":\"" + Provider.serverID + "\"," +
                    "\"Name\":\"" + Provider.serverName + "\"," +
                    "\"Connect\":\"" + Configuration.Instance.ConnectionAddress + "\"," +
                    "\"Map\":\"" + Provider.map + "\"," +
                    "\"MaxPlayers\":\"" + Provider.maxPlayers + "\"," +
                    "\"PlayersOnline\":\"" + Provider.clients.Count + "\"," +
                    "\"LastRefresh\":\"" + DateTime.Now + "\"," +
                    "\"Time\":\"" + LightingManager.time + "\"," +
                    "\"Cycle\":\"" + LightingManager.cycle + "\"," +
                    "\"FullMoon\":\"" + LightingManager.isFullMoon + "\"" +
                    "}";
        }

        public string returnJSONPlayers()
        {
            string json = "";
            int count = 0;

            json += ",\"Players\": {";
            foreach (KeyValuePair<CSteamID, Nodes> Node in Nodes)
            {
                count++;

                json += "\"" + Node.Value.SteamID + "\": {" +
                        "\"CharacterName\":\"" + Node.Value.CharacterName + "\"," +
                        "\"Position\":\"" + Node.Value.Position + "\"," +
                        "\"Reputation\":\"" + Node.Value.Reputation + "\"," +
                        "\"Avatar\":\"" + Node.Value.Avatar + "\"," +
                        "\"Face\":\"" + Node.Value.Face + "\"," +
                        "\"IsInVehicle\":\"" + Node.Value.IsInVehicle + "\"," +
                        "\"VanishMode\":\"" + Node.Value.VanishMode + "\"," +
                        "\"GodMode\":\"" + Node.Value.GodMode + "\"," +
                        "\"IsAdmin\":\"" + Node.Value.IsAdmin + "\"," +
                        "\"Dead\":\"" + Node.Value.Dead + "\"," +
                        "\"Health\":\"" + Node.Value.Health + "\"," +
                        "\"Hunger\":\"" + Node.Value.Hunger + "\"," +
                        "\"Thirst\":\"" + Node.Value.Thirst + "\"," +
                        "\"Infection\":\"" + Node.Value.Infection + "\"," +
                        "\"Stamina\":\"" + Node.Value.Stamina + "\"," +
                        "\"Bleeding\":\"" + Node.Value.Bleeding + "\"," +
                        "\"Broken\":\"" + Node.Value.Broken + "\"," +
                        "\"Hidden\":\"" + Node.Value.Hidden + "\"" +
                        "}";

                if (count < Nodes.Count)
                    json += ",";

            }
            json += "}";

            return json;
        }

        public string returnJSONChat()
        {
            string json = "";
            int count = 0;

            json += ",\"Chat\": {";
            foreach (KeyValuePair<int, Chat> Message in Chat)
            {
                count++;

                json += "\"" + Message.Key + "\": {" +
                    "\"SteamID\":\"" + Message.Value.SteamID + "\"," +
                    "\"CharacterName\":\"" + Message.Value.CharacterName + "\"," +
                    "\"Avatar\":\"" + Message.Value.Avatar + "\"," +
                    "\"isAdmin\":\"" + Message.Value.IsAdmin + "\"," +
                    "\"Message\":\"" + Message.Value.Message + "\"" +
                    "}";

                if (count < Chat.Count)
                    json += ",";
            }
            json += "}";

            Chat.Clear();

            return json;
        }

        public static class ColorTypeConverter
        {
            public static string ToRGBHex(Color c) => $"#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}";

            private static byte ToByte(float f) => (byte)(Mathf.Clamp01(f) * 255);
        }
    } 
}