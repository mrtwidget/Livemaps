using System;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System.Collections.Generic;

namespace NEXIS.Livemap
{
    public class Livemap : RocketPlugin<LivemapConfiguration>
    {
        public static Livemap Instance;
        public DateTime? TimeSinceRefresh = null;
        // public DatabaseManager Database;
        public DataQueue DataQueue;
        public static Dictionary<string, bool> HiddenPlayers = new Dictionary<string, bool>();

        protected override void Load()
        {
            Livemap.Instance = this;

            // Database = new DatabaseManager();
            DataQueue = new DataQueue();

            Logger.Log("Nexis Realms Livemap plugin loaded!", ConsoleColor.Green);
        }

        protected override void Unload()
        {
            Logger.Log("Nexis Realms Livemap plugin unloaded!", ConsoleColor.Yellow);
        }

        public void FixedUpdate()
        {
            // Refresh Livemap Data
            Livemap.Instance.DataQueue.RefreshLivemap();
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"livemap_hidden_cooldown", "You must wait {0} seconds before you can hide again!"},
                    {"livemap_command_usage", "Usage: /livemap"},
                    {"livemap_command_disabled", "The ability to hide from the website livemap is disabled!"},
                    {"livemap_hidden_status", "You are currently {0} from the website livemap! <nexisrealms.com>"}
                };
            }
        }

    }
        
}
