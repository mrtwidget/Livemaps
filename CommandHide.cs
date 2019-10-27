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

        public string Name => "hide";

        public string Help => "This command toggles hiding your character location from the website Livemap.";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/hide";

        public List<string> Permissions => new List<string>() { "hide" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (Livemap.Instance.Configuration.Instance.PlayerHidingEnabled)
            {
                // toggle player hide
                if (Livemap.Instance.Nodes[player.CSteamID].Hidden)
                {
                    Livemap.Instance.Nodes[player.CSteamID].Hidden = false;
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_false"), Color.yellow);
                }
                else
                {
                    Livemap.Instance.Nodes[player.CSteamID].Hidden = true;
                    UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_true"), Color.yellow);
                }                    
            }
            else
                UnturnedChat.Say(caller, Livemap.Instance.Translations.Instance.Translate("livemap_hidden_disabled"), Color.red);
        }
    }
}