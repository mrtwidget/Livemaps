using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Livemap
{
    public class CommandLivemap : IRocketCommand
    {
        #region Properties
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "livemap";

        public string Help => "This command toggles hiding your character location from the website Livemap.";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/livemap";

        public List<string> Permissions => new List<string>() { "livemap" };

        #endregion
        
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (Livemap.Instance.Configuration.Instance.PlayerHideEnabled)
            {
                // if player is in cooldown state
                if (Livemap.Instance.PlayersHiddenCooldown.ContainsKey(player.CSteamID))
                {
                    UnturnedChat.Say(caller, Massive(player), Color.yellow);
                    
                    return;
                }

                // if player is currently hiding
                if (Livemap.Instance.PlayersHidden.ContainsKey(player.CSteamID))
                    PlayerHidden(player, caller);
                else // player is not hiding
                    PlayerVisible(player, caller);
            }
            else
            {
                // command disabled in config
                UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_disabled"), Color.red);
            }
        }

        public void PlayerHidden(UnturnedPlayer player, IRocketPlayer caller)
        {
            // save player hide duration
            Livemap.Instance.PlayersHiddenDuration.Add(player.CSteamID, (DateTime.Now - Livemap.Instance.PlayersHidden[player.CSteamID].Value).TotalSeconds);

            // unhide player
            Livemap.Instance.PlayersHidden.Remove(player.CSteamID);
                    
            UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_false"), Color.yellow);
        }

        public void PlayerVisible(UnturnedPlayer player, IRocketPlayer caller)
        {
            double savedDuration = Livemap.Instance.Configuration.Instance.PlayerHideDuration;

            // add previous player hide duration
            if (Livemap.Instance.PlayersHiddenDuration.ContainsKey(player.CSteamID))
            {
                savedDuration = Livemap.Instance.PlayersHiddenDuration[player.CSteamID];

                Livemap.Instance.PlayersHidden.Add(player.CSteamID, DateTime.Now.AddSeconds(-savedDuration));
                Livemap.Instance.PlayersHiddenDuration.Remove(player.CSteamID); // remove player

                TimeSpan span = DateTime.Now.Subtract(DateTime.Now.AddSeconds(-savedDuration));
                int result = Livemap.Instance.Configuration.Instance.PlayerHideDuration - span.Seconds;

                UnturnedChat.Say(caller,
                    Livemap.Instance.Translations.Instance.Translate("livemap_hidden_true",
                        result + " seconds"), Color.white);
            }
            else
            {
                Livemap.Instance.PlayersHidden.Add(player.CSteamID, DateTime.Now);
                UnturnedChat.Say(caller,
                    Livemap.Instance.Translations.Instance.Translate("livemap_hidden_true",
                        savedDuration + " seconds"), Color.white);
            }
        }
        
        string Massive(UnturnedPlayer player) => Livemap.Instance.Translations.Instance.Translate("livemap_hidden_cooldown_remaining",
            (Math.Round(
                (Livemap.Instance.Configuration.Instance.PlayerHideCooldownDuration -
                 (DateTime.Now - (DateTime) Livemap.Instance.PlayersHiddenCooldown[player.CSteamID])
                 .TotalSeconds), 0, MidpointRounding.AwayFromZero)) + " seconds");
    }
}