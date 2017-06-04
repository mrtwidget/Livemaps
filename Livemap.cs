using Logger = Rocket.Core.Logging.Logger;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.API;
using UnityEngine;

namespace NEXIS.Livemap
{
    public class Livemap : RocketPlugin<LivemapConfiguration>
    {
        public static Livemap Instance;
        public DatabaseManager Database;

        // last refresh timestamp
        public DateTime? LastRefresh = null;
        // <Steam64ID, CharacterName>
        public static Dictionary<CSteamID, string> PlayersOnline = new Dictionary<CSteamID, string>();
        // <Steam64ID, timestamp hidden>
        public static Dictionary<CSteamID, DateTime?> PlayersHidden = new Dictionary<CSteamID, DateTime?>();

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead += Events_OnPlayerDead;
            UnturnedPlayerEvents.OnPlayerDeath += Events_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerRevive += Events_OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerChatted += Events_OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerUpdateExperience += Events_OnPlayerUpdateExperience;

            Logger.Log("Livemaps successfully loaded!", ConsoleColor.Green);
        }

        protected override void Unload()
        {
            // update all player `last_disconnect` columns
            Livemap.Instance.Database.Unload();

            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead -= Events_OnPlayerDead;
            UnturnedPlayerEvents.OnPlayerDeath -= Events_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerRevive -= Events_OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerChatted -= Events_OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerUpdateExperience -= Events_OnPlayerUpdateExperience;

            Logger.Log("Livemaps successfully unloaded!", ConsoleColor.Green);
        }

        private bool IsPlayerHidden(UnturnedPlayer player)
        {
            if (Livemap.PlayersHidden.ContainsKey(player.CSteamID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void FixedUpdate()
        {
            if (this.State == PluginState.Loaded && Livemap.Instance.LastRefresh == null || (DateTime.Now - this.LastRefresh.Value).TotalSeconds > Livemap.Instance.Configuration.Instance.RefreshInterval)
            {
                // refresh server data
                Livemap.Instance.Database.RefreshServer();

                // loop through each player and update the database
                foreach (SteamPlayer player in Provider.clients)
                {
                    // refresh player data
                    if (player == null) { continue; }
                    Livemap.Instance.Database.RefreshPlayer(UnturnedPlayer.FromSteamPlayer(player));
                }

                // update refresh timestamp
                Livemap.Instance.LastRefresh = DateTime.Now;
            }
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"livemap_hidden_cooldown", "You must wait {0} before you can hide again!"},
                    {"livemap_command_usage", "Usage: Type /livemap to toggle hiding"},
                    {"livemap_hidding_disabled", "Livemap hiding has been disabled!"},
                    {"livemap_hidden_true", "You are now hidden from the livemap."},
                    {"livemap_hidden_false", "You are now visible on the livemap."}
                };
            }
        }


        public void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            // player connected
            Livemap.Instance.Database.OnPlayerConnected(player);
        }

        public void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            // player disconnected
            Livemap.Instance.Database.OnPlayerDisconnected(player);
        }

        public void Events_OnPlayerDead(UnturnedPlayer player, Vector3 position)
        {
            // save player corpse position
            Livemap.Instance.Database.OnPlayerDead(player, position);
        }

        public void Events_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            // save kill data
            //Livemap.Instance.Database.OnPlayerDeath(player, cause, limb, murderer);
        }

        public void Events_OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
            // player respawned
            Livemap.Instance.Database.OnPlayerRevive(player, position, angle);
        }

        public void Events_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            // save world chat
            Livemap.Instance.Database.OnPlayerChatted(player, ref color, message, chatMode);
        }

        public void Events_OnPlayerUpdateExperience(UnturnedPlayer player, uint experience)
        {
            // player reputation increased
            Livemap.Instance.Database.OnPlayerUpdateExperience(player, experience);
        }

    }
        
}
