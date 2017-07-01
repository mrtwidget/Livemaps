using Logger = Rocket.Core.Logging.Logger;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
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
        // Online Players <Steam64ID, CharacterName>
        public static Dictionary<CSteamID, string> PlayersOnline = new Dictionary<CSteamID, string>();
        // Hidden Players <Steam64ID, DateTime>
        public static Dictionary<CSteamID, DateTime?> PlayersHidden = new Dictionary<CSteamID, DateTime?>();
        // Hide Cooldown <Steam64ID, DateTime>
        public static Dictionary<CSteamID, DateTime?> PlayersHiddenCooldown = new Dictionary<CSteamID, DateTime?>();
        // Player Hide Duration <Steam64ID, DateTime>
        public static Dictionary<CSteamID, double> PlayersHiddenDuration = new Dictionary<CSteamID, double>();

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead += Events_OnPlayerDead;
            UnturnedPlayerEvents.OnPlayerRevive += Events_OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerChatted += Events_OnPlayerChatted;

            Logger.Log("Livemaps successfully loaded!", ConsoleColor.Green);
        }

        protected override void Unload()
        {
            // update all player `last_disconnect` columns
            Livemap.Instance.Database.CleanUp();

            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead -= Events_OnPlayerDead;
            UnturnedPlayerEvents.OnPlayerRevive -= Events_OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerChatted -= Events_OnPlayerChatted;

            Logger.Log("Livemaps successfully unloaded!", ConsoleColor.Green);
        }

        public int IsPlayerHidden(UnturnedPlayer player)
        {
            if (Livemap.PlayersHidden.ContainsKey(player.CSteamID))
            {
                return 1; // true
            }
            else
            {
                return 0; // false
            }
        }

        public void FixedUpdate()
        {
            if (this.State == PluginState.Loaded) {

                /* Update Database */
                if (Livemap.Instance.LastRefresh == null || (DateTime.Now - this.LastRefresh.Value).TotalSeconds > Livemap.Instance.Configuration.Instance.LivemapRefreshInterval)
                {
                    // refresh server database data
                    Livemap.Instance.Database.UpdateServerData();

                    // update each connected player in the database
                    Livemap.Instance.Database.UpdateAllPlayers();

                    // update refresh timestamp
                    Livemap.Instance.LastRefresh = DateTime.Now;
                }

                /* Update Player Cooldown */
                if (Livemap.Instance.Configuration.Instance.PlayerHideCooldownEnabled) {

                    /* Livemap Hide Cooldown Expiration */
                    Dictionary<CSteamID, DateTime?> cooldownHiddenPlayers = new Dictionary<CSteamID, DateTime?>(Livemap.PlayersHiddenCooldown);
                    foreach (KeyValuePair<CSteamID, DateTime?> cooldownPlr in cooldownHiddenPlayers)
                    {
                        /* Player Cooldown Expired */
                        if ((DateTime.Now - (DateTime)cooldownPlr.Value).TotalSeconds > Livemap.Instance.Configuration.Instance.PlayerHideCooldownDuration)
                        {
                            // remove player cooldown
                            Livemap.PlayersHiddenCooldown.Remove(cooldownPlr.Key);

                            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(cooldownPlr.Key), Livemap.Instance.Translations.Instance.Translate("livemap_hidden_cooldown_end"));
                        }
                    }

                    /* Player Hide Expiration */
                    Dictionary<CSteamID, DateTime?> hiddenPlayers = new Dictionary<CSteamID, DateTime?>(Livemap.PlayersHidden);
                    foreach (KeyValuePair<CSteamID, DateTime?> hiddenPlr in hiddenPlayers)
                    {
                        /* Player Hide Expired */
                        if ((DateTime.Now - (DateTime)hiddenPlr.Value).TotalSeconds > Livemap.Instance.Configuration.Instance.PlayerHideDuration)
                        {
                            // add player to cooldown
                            Livemap.PlayersHiddenCooldown.Add(hiddenPlr.Key, DateTime.Now);

                            // unhide player
                            Livemap.PlayersHidden.Remove(hiddenPlr.Key);

                            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(hiddenPlr.Key), Livemap.Instance.Translations.Instance.Translate("livemap_hidden_cooldown_start", Livemap.Instance.Configuration.Instance.PlayerHideCooldownDuration + " seconds"));
                        }
                    }
                }
            }
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"livemap_command_usage", "Usage: Type: /livemap to toggle hiding yourself from the Livemap."},
                    {"livemap_hidden_disabled", "Livemap hiding is currently disabled! :("},
                    {"livemap_hidden_true", "Your location is hidden on the Livemap for the next {0}!"},
                    {"livemap_hidden_false", "Your location is now visible to anyone on the Livemap!"},
                    {"livemap_hidden_cooldown", "You must wait {0} before you can hide your location on the Livemap again!"},
                    {"livemap_hidden_cooldown_end", "Livemap cooldown expired! You can hide your location again!!"},
                    {"livemap_hidden_cooldown_start", "Livemap hiding expired! You must wait {0} to hide again!"},
                    {"livemap_hidden_cooldown_remaining", "You must wait {0} before you can hide yourself on the Livemap again!"}
                };
            }
        }

        /* Player Connected */
        public void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            // update player data
            Livemap.Instance.Database.OnPlayerConnected(player);
        }

        /* Player Disconnect */
        public void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            // remove player from hidden dictionary
            if (Livemap.PlayersHidden.ContainsKey(player.CSteamID))
            {
                Livemap.PlayersHidden.Remove(player.CSteamID);
            }

            // remove player from cooldown dictionary
            if (Livemap.PlayersHiddenCooldown.ContainsKey(player.CSteamID))
            {
                Livemap.PlayersHiddenCooldown.Remove(player.CSteamID);
            }

            // update player `last_disconnect`
            Livemap.Instance.Database.OnPlayerDisconnected(player);
        }

        public void Events_OnPlayerDead(UnturnedPlayer player, Vector3 position)
        {
            // save player corpse position
            Livemap.Instance.Database.OnPlayerDead(player, position);
        }

        public void Events_OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
            // player respawned, update dead status
            Livemap.Instance.Database.OnPlayerRevive(player, position, angle);
        }

        public void Events_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            // save world chat
            Livemap.Instance.Database.OnPlayerChatted(player, ref color, message, chatMode);
        }

    }
        
}
