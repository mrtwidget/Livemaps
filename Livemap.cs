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
        #region Fields
        
        public static Livemap Instance;
        public DatabaseManager Database;

        // last refresh timestamp
        public DateTime? LastRefresh;
        
        // Online Players <Steam64ID, CharacterName>
        public Dictionary<CSteamID, string> PlayersOnline;
        
        // Hidden Players <Steam64ID, DateTime>
        public Dictionary<CSteamID, DateTime?> PlayersHidden;
        
        // Hide Cooldown <Steam64ID, DateTime>
        public Dictionary<CSteamID, DateTime?> PlayersHiddenCooldown;
        
        // Player Hide Duration <Steam64ID, DateTime>
        public Dictionary<CSteamID, double> PlayersHiddenDuration;
        
        #endregion
        
        #region Overrides

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            
            PlayersOnline = new Dictionary<CSteamID, string>();
            PlayersHidden = new Dictionary<CSteamID, DateTime?>();
            PlayersHiddenCooldown = new Dictionary<CSteamID, DateTime?>();
            PlayersHiddenDuration = new Dictionary<CSteamID, double>();

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
            Database.CleanUp();

            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead -= Events_OnPlayerDead;
            UnturnedPlayerEvents.OnPlayerRevive -= Events_OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerChatted -= Events_OnPlayerChatted;

            Logger.Log("Livemaps successfully unloaded!", ConsoleColor.Green);
        }
        
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"livemap_command_usage", "Usage: Type: /livemap to toggle hiding yourself from the Livemap."},
            {"livemap_hidden_disabled", "Livemap hiding is currently disabled! :("},
            {"livemap_hidden_true", "Your location is hidden on the Livemap for the next {0}!"},
            {"livemap_hidden_false", "Your location is now visible to anyone on the Livemap!"},
            {"livemap_hidden_cooldown", "You must wait {0} before you can hide your location on the Livemap again!"},
            {"livemap_hidden_cooldown_end", "Livemap cooldown expired! You can hide your location again!!"},
            {"livemap_hidden_cooldown_start", "Livemap hiding expired! You must wait {0} to hide again!"},
            {"livemap_hidden_cooldown_remaining", "You must wait {0} before you can hide yourself on the Livemap again!"}
        };
        
        #endregion
        
        #region Events

        /* Player Connected */
        public void Events_OnPlayerConnected(UnturnedPlayer player) => Database.OnPlayerConnected(player);

        /* Player Disconnect */
        public void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            // remove player from hidden dictionary
            if (PlayersHidden.ContainsKey(player.CSteamID))
                PlayersHidden.Remove(player.CSteamID);

            // remove player from cooldown dictionary
            if (PlayersHiddenCooldown.ContainsKey(player.CSteamID))
                PlayersHiddenCooldown.Remove(player.CSteamID);

            // update player `last_disconnect`
            Database.OnPlayerDisconnected(player);
        }

        // Saves player corpose position
        public void Events_OnPlayerDead(UnturnedPlayer player, Vector3 position) => Instance.Database.OnPlayerDead(player, position);

        // player respawned, update dead status
        public void Events_OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle) => Database.OnPlayerRevive(player, position, angle);

        // save world chat
        public void Events_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode,
            ref bool cancel) => Database.OnPlayerChatted(player, ref color, message, chatMode);

        #endregion

        public void FixedUpdate()
        {
            if (Instance.State != PluginState.Loaded) return;
            
            /* Update Database */
            if (Instance.LastRefresh == null || (DateTime.Now - Instance.LastRefresh.Value).TotalSeconds > Instance.Configuration.Instance.LivemapRefreshInterval)
                UpdateDatabase();

            /* Update Player Cooldown */
            if (!Instance.Configuration.Instance.PlayerHideCooldownEnabled) return;
            
            /* Livemap Hide Cooldown Expiration */
            HideCooldownExpiration();

            /* Player Hide Expiration */
            PlayerHideExpiration();
        }

        public void UpdateDatabase()
        {
            // refresh server database data
            Instance.Database.UpdateServerData();

            // update each connected player in the database
            Instance.Database.UpdateAllPlayers();

            // update refresh timestamp
            Instance.LastRefresh = DateTime.Now;
        }

        public void HideCooldownExpiration()
        {
            foreach (KeyValuePair<CSteamID, DateTime?> cooldownPlr in PlayersHiddenCooldown)
            {
                /* If player cooldown not expired, skip player */
                if (!((DateTime.Now - (DateTime) cooldownPlr.Value).TotalSeconds >
                      Instance.Configuration.Instance.PlayerHideCooldownDuration)) continue;
                
                // remove player cooldown
                Instance.PlayersHiddenCooldown.Remove(cooldownPlr.Key);

                UnturnedChat.Say(UnturnedPlayer.FromCSteamID(cooldownPlr.Key), Instance.Translations.Instance.Translate("livemap_hidden_cooldown_end"));
            }
        }

        public void PlayerHideExpiration()
        {
            foreach (KeyValuePair<CSteamID, DateTime?> hiddenPlr in PlayersHidden)
            {
                /* If player hide not expired, skip player */
                if (!((DateTime.Now - (DateTime) hiddenPlr.Value).TotalSeconds >
                      Configuration.Instance.PlayerHideDuration)) continue;
                
                // add player to cooldown
                PlayersHiddenCooldown.Add(hiddenPlr.Key, DateTime.Now);

                // unhide player
                PlayersHidden.Remove(hiddenPlr.Key);

                UnturnedChat.Say(UnturnedPlayer.FromCSteamID(hiddenPlr.Key), Translations.Instance.Translate("livemap_hidden_cooldown_start", Configuration.Instance.PlayerHideCooldownDuration + " seconds"));
            }
        }
        
        public bool IsPlayerHidden(UnturnedPlayer player) => Instance.PlayersHidden.ContainsKey(player.CSteamID);
    }
        
}
