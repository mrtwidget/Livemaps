using Logger = Rocket.Core.Logging.Logger;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (Livemap.Instance.LastRefresh == null || ((DateTime.Now - Livemap.Instance.LastRefresh.Value).TotalSeconds > Livemap.Instance.Configuration.Instance.RefreshInterval))
            {
                /* Server Data */
                ServerData server = new ServerData(
                    Provider.serverID, Provider.serverName, Provider.APP_VERSION, Provider.map, Provider.clients.Count, Provider.maxPlayers, Provider.isPvP, Provider.isGold, 
                    Provider.hasCheats, Provider.hideAdmins, LightingManager.time, LightingManager.cycle, LightingManager.isFullMoon, Time.time, Provider.packetsReceived,
                    Provider.packetsSent, Provider.port, Provider.mode
                );

                /* Player Data */
                PlayerData[] players = new PlayerData[Provider.clients.Count];
                int i = 0; // index

                foreach (SteamPlayer player in Provider.clients)
                {
                    // Convert SteamPlayer into UnturnedPlayer
                    UnturnedPlayer plr = UnturnedPlayer.FromSteamPlayer(player);

                    // Gather player details
                    players[i] = new PlayerData(
                        plr.CSteamID,
                        plr.CharacterName, plr.DisplayName,
                        plr.SteamGroupID, plr.SteamProfile.AvatarMedium, plr.SteamProfile.Headline,
                        plr.IP, plr.Ping,
                        plr.IsPro, plr.IsAdmin, plr.GodMode, plr.VanishMode,
                        plr.IsInVehicle,
                            plr.CurrentVehicle.enabled,
                            plr.CurrentVehicle.isDriver,
                            plr.CurrentVehicle.instanceID,
                            plr.CurrentVehicle.id,
                            plr.CurrentVehicle.fuel,
                            plr.CurrentVehicle.health,
                            plr.CurrentVehicle.headlightsOn,
                            plr.CurrentVehicle.taillightsOn,
                            plr.CurrentVehicle.sirensOn,
                            plr.CurrentVehicle.speed,
                            plr.CurrentVehicle.hasBattery,
                            plr.CurrentVehicle.batteryCharge,
                            plr.CurrentVehicle.isExploded,
                            plr.CurrentVehicle.isLocked,
                            plr.CurrentVehicle.lockedOwner,
                        plr.Position, plr.Rotation,                        
                        plr.Dead,
                        plr.Player.clothing.skin,
                        plr.Player.clothing.hair,
                        plr.Player.clothing.face,
                        plr.Player.clothing.beard,
                        plr.Player.clothing.visualHat,
                        plr.Player.clothing.visualGlasses,
                        plr.Player.clothing.visualMask,
                        plr.Bleeding, plr.Broken,
                        plr.Health, plr.Stamina, plr.Hunger, plr.Thirst, plr.Infection,
                        plr.Experience, plr.Reputation,
                        plr.GetSkillLevel(UnturnedSkill.Agriculture),
                        plr.GetSkillLevel(UnturnedSkill.Cardio),
                        plr.GetSkillLevel(UnturnedSkill.Cooking),
                        plr.GetSkillLevel(UnturnedSkill.Crafting),
                        plr.GetSkillLevel(UnturnedSkill.Dexerity),
                        plr.GetSkillLevel(UnturnedSkill.Diving),
                        plr.GetSkillLevel(UnturnedSkill.Engineer),
                        plr.GetSkillLevel(UnturnedSkill.Exercise),
                        plr.GetSkillLevel(UnturnedSkill.Fishing),
                        plr.GetSkillLevel(UnturnedSkill.Healing),
                        plr.GetSkillLevel(UnturnedSkill.Immunity),
                        plr.GetSkillLevel(UnturnedSkill.Mechanic),
                        plr.GetSkillLevel(UnturnedSkill.Outdoors),
                        plr.GetSkillLevel(UnturnedSkill.Overkill),
                        plr.GetSkillLevel(UnturnedSkill.Parkour),
                        plr.GetSkillLevel(UnturnedSkill.Sharpshooter),
                        plr.GetSkillLevel(UnturnedSkill.Sneakybeaky),
                        plr.GetSkillLevel(UnturnedSkill.Strength),
                        plr.GetSkillLevel(UnturnedSkill.Survival),
                        plr.GetSkillLevel(UnturnedSkill.Toughness),
                        plr.GetSkillLevel(UnturnedSkill.Vitality),
                        plr.GetSkillLevel(UnturnedSkill.Warmblooded),
                        IsPlayerHidden(plr)
                    );

                    ++i; // increment index
                }
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
            Livemap.Instance.Database.OnPlayerDeath(player, cause, limb, murderer);
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
