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
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }
        public bool AllowFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "livemap"; }
        }

        public string Help
        {
            get { return "This command toggles hiding your character location from the website Livemap."; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { }; }
        }

        public string Syntax
        {
            get { return "/livemap"; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "livemap" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (Livemap.Instance.Configuration.Instance.PlayerHideEnabled)
            {
                // if player is in cooldown state
                if (Livemap.PlayersHiddenCooldown.ContainsKey(player.CSteamID)) {
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_cooldown_remaining", (Math.Round((Livemap.Instance.Configuration.Instance.PlayerHideCooldownDuration - (DateTime.Now -(DateTime)Livemap.PlayersHiddenCooldown[player.CSteamID]).TotalSeconds), 0, MidpointRounding.AwayFromZero)) + " seconds"), Color.yellow);
                    return;
                }

                // if player is currently hiding
                if (Livemap.PlayersHidden.ContainsKey(player.CSteamID))
                {
                    // save player hide duration
                    Livemap.PlayersHiddenDuration.Add(player.CSteamID, (DateTime.Now - Livemap.PlayersHidden[player.CSteamID].Value).TotalSeconds);

                    // unhide player
                    Livemap.PlayersHidden.Remove(player.CSteamID);
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_false"), Color.yellow);
                }
                else // player is not hiding
                {
                    double savedDuration = Livemap.Instance.Configuration.Instance.PlayerHideDuration;

                    // add previous player hide duration
                    if (Livemap.PlayersHiddenDuration.ContainsKey(player.CSteamID))
                    {
                        savedDuration = Livemap.PlayersHiddenDuration[player.CSteamID];

                        Livemap.PlayersHidden.Add(player.CSteamID, DateTime.Now.AddSeconds(-savedDuration));
                        Livemap.PlayersHiddenDuration.Remove(player.CSteamID); // remove player

                        TimeSpan span = DateTime.Now.Subtract(DateTime.Now.AddSeconds(-savedDuration));
                        int result = Livemap.Instance.Configuration.Instance.PlayerHideDuration - span.Seconds;

                        UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_true", result + " seconds"), Color.white);
                    }
                    else
                    {
                        Livemap.PlayersHidden.Add(player.CSteamID, DateTime.Now);
                        UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_true", savedDuration + " seconds"), Color.white);
                    }
                }
            }
            else
            {
                // command disabled in config
                UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_disabled"), Color.red);
            }
        }
    }
}