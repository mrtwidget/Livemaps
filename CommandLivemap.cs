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

        // Run the command.
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            // if livemap hide is enabled in config, toggle player [hide|show]
            if (Livemap.Instance.Configuration.Instance.EnableHiding)
            {
                // is player in the PlayersHidden dictionary?
                if (Livemap.PlayersHidden.ContainsKey(player.CSteamID))
                {
                    Livemap.PlayersHidden.Remove(player.CSteamID);
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_false"));
                }
                else
                {
                    Livemap.PlayersHidden.Add(player.CSteamID, DateTime.Now);
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_true"));
                }
            }
            else
            {
                // livemap command disabled in config, notify player
                UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidding_disabled"));
            }
        }
    }
}