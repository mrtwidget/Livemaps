using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

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
            get { return "Toggle hiding yourself from the website livemap."; }
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

            // if player hiding is enabled, toggle player hidden
            if (Livemap.Instance.Configuration.Instance.PlayerHideEnabled)
            {
                // is player in PlayersHiddenCooldown dictionary?
                if (Livemap.PlayersHiddenCooldown.ContainsKey(player.CSteamID)) {
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_cooldown_remaining", (Math.Round((Livemap.Instance.Configuration.Instance.PlayerHideCooldownDuration - (DateTime.Now -(DateTime)Livemap.PlayersHiddenCooldown[player.CSteamID]).TotalSeconds), 0, MidpointRounding.AwayFromZero)) + " seconds"));
                    return;
                }

                // is player already hiding?
                if (Livemap.PlayersHidden.ContainsKey(player.CSteamID))
                {
                    // hiding; unhide player
                    Livemap.PlayersHidden.Remove(player.CSteamID);
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_false"));
                }
                else
                {
                    // not hiding; hide player
                    Livemap.PlayersHidden.Add(player.CSteamID, DateTime.Now);
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_true", Livemap.Instance.Configuration.Instance.PlayerHideDuration + " seconds"));
                }
            }
            else
            {
                // livemap command disabled in config, notify player
                UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_disabled"));
            }
        }
    }
}