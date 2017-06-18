using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using SDG.Unturned;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace NEXIS.Livemap
{
    public class DatabaseManager
    {
        /**
         * Construct Class
         * 
         * This function includes required database DLL and checks the schema
         */
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        /**
         * Check Schema / Create Database Tables
         * 
         * This function checks if the required database tables already exist in the database and creates
         * them if they do not already exist.
         */
        public void CheckSchema()
        {
            /* `livemap_server` table */
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // table doesn't exist, create it
                    MySQLCommand.CommandText = "CREATE TABLE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer +
                    "(server_id VARCHAR(32) NOT NULL," +
                    "server_name VARCHAR(50) NULL," +
                    "app_version VARCHAR(32) NULL," +
                    "map VARCHAR(32) NULL," +
                    "online_players INT(8) NULL," +
                    "max_players INT(8) NULL," +
                    "is_pvp TINYINT(1) NULL," +
                    "is_gold TINYINT(1) NULL," +
                    "is_pro TINYINT(1) NULL," +
                    "has_cheats TINYINT(1) NULL," +
                    "hide_admins TINYINT(1) NULL," +
                    "cycle_time INT(8) NULL," +
                    "cycle_length INT(8) NULL," +
                    "full_moon TINYINT(1) NULL," +
                    "uptime INT(10) NULL," +
                    "packets_received INT(10) NULL," +
                    "packets_sent INT(10) NULL," +
                    "port INT(8) NULL," +
                    "mode VARCHAR(32) NULL," +
                    "last_refresh TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                    "PRIMARY KEY(server_id));";

                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            /* `livemap_data` table */
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // table doesn't exist, create it!
                    MySQLCommand.CommandText = "CREATE TABLE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + 
                    "(CSteamID VARCHAR(32) NOT NULL," +
                    "character_name VARCHAR(100) NULL," +
                    "display_name VARCHAR(100) NULL," +
                    "steam_group_id VARCHAR(32) NULL," +
                    "steam_avatar_medium VARCHAR(255) NULL," +
                    "server_id VARCHAR(32) NULL," +
                    "ip_address VARCHAR(255) NULL," +
                    "ping DECIMAL(8,6) NULL," +
                    "is_pro TINYINT(1) NULL," +
                    "is_admin TINYINT(1) NULL," +
                    "is_god TINYINT(1) NULL," +
                    "is_vanished TINYINT(1) NULL," +
                    "in_vehicle TINYINT(1) NULL," +
                    "vehicle_is_driver TINYINT(1) NULL," +
                    "vehicle_instance_id INT(8) NULL," +
                    "vehicle_id INT(8) NULL," +
                    "vehicle_fuel INT(8) NULL," +
                    "vehicle_health INT(8) NULL," +
                    "vehicle_headlights_on TINYINT(1) NULL," +
                    "vehicle_taillights_on TINYINT(1) NULL," +
                    "vehicle_sirens_on TINYINT(1) NULL," +
                    "vehicle_speed INT(8) NULL," +
                    "vehicle_has_battery TINYINT(1) NULL," +
                    "vehicle_battery_charge INT(8) NULL," +
                    "vehicle_exploded TINYINT(1) NULL," +
                    "vehicle_locked TINYINT(1) NULL," +                            
                    "position VARCHAR(32) NULL," +
                    "rotation FLOAT(7,4) NULL," +
                    "is_dead TINYINT(1) NULL," +
                    "last_dead_position VARCHAR(32) NULL," +
                    "skin_color VARCHAR(32) NULL," +
                    "hair INT(8) NULL," +
                    "face INT(8) NULL," +
                    "beard INT(8) NULL," +
                    "hat INT(8) NULL," +
                    "glasses INT(8) NULL," +
                    "mask INT(8) NULL," +
                    "is_bleeding TINYINT(1) NULL," +
                    "is_broken TINYINT(1) NULL," +
                    "health INT(8) NULL," +
                    "stamina INT(8) NULL," +
                    "hunger INT(8) NULL," +
                    "thirst INT(8) NULL," +
                    "infection INT(8) NULL," +
                    "experience INT(8) NULL," +
                    "reputation INT(8) NULL," +
                    "skill_agriculture INT(8) NULL," +
                    "skill_cardio INT(8) NULL," +
                    "skill_cooking INT(8) NULL," +
                    "skill_crafting INT(8) NULL," +
                    "skill_dexerity INT(8) NULL," +
                    "skill_diving INT(8) NULL," +
                    "skill_engineer INT(8) NULL," +
                    "skill_exercise INT(8) NULL," +
                    "skill_fishing INT(8) NULL," +
                    "skill_healing INT(8) NULL," +
                    "skill_immunity INT(8) NULL," +
                    "skill_mechanic INT(8) NULL," +
                    "skill_outdoors INT(8) NULL," +
                    "skill_overkill INT(8) NULL," +
                    "skill_parkour INT(8) NULL," +
                    "skill_sharpshooter INT(8) NULL," +
                    "skill_sneakybeaky INT(8) NULL," +
                    "skill_strength INT(8) NULL," +
                    "skill_survival INT(8) NULL," +
                    "skill_toughness INT(8) NULL," +
                    "skill_vitality INT(8) NULL," +
                    "skill_warmblooded INT(8) NULL," +
                    "is_hidden TINYINT(1) DEFAULT 0," +
                    "last_refresh TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                    "last_connect TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                    "last_disconnect TIMESTAMP NULL," +
                    "PRIMARY KEY(CSteamID));";
                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            
            /* `livemap_chat` table */
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapChat + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // table doesn't exist, create it
                    MySQLCommand.CommandText = "CREATE TABLE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapChat +
                    "(id INT(8) NOT NULL AUTO_INCREMENT," +
                    "server_id VARCHAR(32) NULL," +
                    "steam_id VARCHAR(50) NULL," +
                    "message VARCHAR(100) NULL," +
                    "timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                    "PRIMARY KEY(id));";

                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /* mysql data connection */
        private MySqlConnection CreateConnection()
        {
            MySqlConnection MySQLConnection = null;

            try
            {
                if (Livemap.Instance.Configuration.Instance.DatabasePort == 0)
                {
                    Livemap.Instance.Configuration.Instance.DatabasePort = 3306;
                }

                MySQLConnection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", new object[] {
                    Livemap.Instance.Configuration.Instance.DatabaseHost,
                    Livemap.Instance.Configuration.Instance.DatabaseName,
                    Livemap.Instance.Configuration.Instance.DatabaseUser,
                    Livemap.Instance.Configuration.Instance.DatabasePass,
                    Livemap.Instance.Configuration.Instance.DatabasePort
                }));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return MySQLConnection;
        }

        public void RefreshServer()
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if server exists in table
                MySQLCommand.CommandText = "SELECT * FROM " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + " WHERE server_id = '" + Provider.serverID + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // server does not exist, create it
                    MySQLCommand.CommandText = "INSERT INTO " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + " (server_id,server_name,app_version,map,online_players,max_players,is_pvp,is_gold,is_pro,has_cheats,hide_admins,cycle_time,cycle_length,full_moon,uptime,packets_received,packets_sent,port,mode) VALUES (" +
                        "'" + Provider.serverID + "'," +
                        "'" + Provider.serverName + "'," +
                        "'" + Provider.APP_VERSION + "'," +
                        "'" + Provider.map + "'," +
                        Provider.clients.Count + "," +
                        Provider.maxPlayers + "," +
                        Convert.ToInt32(Provider.isPvP) + "," +
                        Convert.ToInt32(Provider.isGold) + "," +
                        Convert.ToInt32(Provider.isPro) + "," +
                        Convert.ToInt32(Provider.hasCheats) + "," +
                        Convert.ToInt32(Provider.hideAdmins) + "," +
                        LightingManager.time + "," +
                        LightingManager.cycle + "," +
                        Convert.ToInt32(LightingManager.isFullMoon) + "," +
                        Time.time + "," +
                        Provider.packetsReceived + "," +
                        Provider.packetsSent + "," +
                        Provider.port + "," +
                        "'" + Provider.mode.ToString() + "'" +
                    ")";

                    MySQLCommand.ExecuteNonQuery();
                }
                else
                {
                    // update server info
                    MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + " SET " +
                    "server_name = '" + Provider.serverName + "'," +
                    "app_version = '" + Provider.APP_VERSION + "'," +
                    "map = '" + Provider.map + "'," +
                    "online_players = " + Provider.clients.Count + "," +
                    "max_players = " + Provider.maxPlayers + "," +
                    "is_pvp = " + Convert.ToInt32(Provider.isPvP) + "," +
                    "is_gold = " + Convert.ToInt32(Provider.isGold) + "," +
                    "is_pro = " + Convert.ToInt32(Provider.isPro) + "," +
                    "has_cheats = " + Convert.ToInt32(Provider.hasCheats) + "," +
                    "hide_admins = " + Convert.ToInt32(Provider.hideAdmins) + "," +
                    "cycle_time = " + LightingManager.time + "," +
                    "cycle_length = " + LightingManager.cycle + "," +
                    "full_moon = " + Convert.ToInt32(LightingManager.isFullMoon) + "," +
                    "uptime = " + Time.time + "," +
                    "packets_received = " + Provider.packetsReceived + "," +
                    "packets_sent = " + Provider.packetsSent + "," +
                    "port = " + Provider.port + "," +
                    "mode = '" + Provider.mode.ToString() + "' " +
                    "WHERE server_id = '" + Provider.serverID + "'";

                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         * Refresh Player Data
         * 
         * This function collects aplayer's current stats and updates the database to reflect this change.
         * @param UnturnedPlayer player The player that will be updated
         */
        public void RefreshPlayer(UnturnedPlayer player)
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if player exists in table
                MySQLCommand.CommandText = "SELECT * FROM " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // player does not exist, create it
                    MySQLCommand.CommandText = "INSERT INTO " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " (CSteamID,character_name,display_name,steam_group_id,steam_avatar_medium,server_id,ip_address,ping,is_pro,is_admin,is_god,is_vanished,in_vehicle,position,rotation,skin_color,hair,face,beard,hat,glasses,mask,is_bleeding,is_broken,health,stamina,hunger,thirst,infection,experience,reputation,skill_agriculture,skill_cardio,skill_cooking,skill_crafting,skill_dexerity,skill_diving,skill_engineer,skill_exercise,skill_fishing,skill_healing,skill_immunity,skill_mechanic,skill_outdoors,skill_overkill,skill_parkour,skill_sharpshooter,skill_sneakybeaky,skill_strength,skill_survival,skill_toughness,skill_vitality,skill_warmblooded,is_hidden) VALUES (" +
                    "'" + player.CSteamID.ToString() + "'," +
                    "'" + player.CharacterName.ToString() + "'," +
                    "'" + player.DisplayName.ToString() + "'," +
                    "'" + player.SteamGroupID.ToString() + "'," +
                    "'" + (player.SteamProfile.AvatarMedium != null ? player.SteamProfile.AvatarMedium.ToString() : Livemap.Instance.Configuration.Instance.PlayerDefaultSteamAvatar) + "'," +
                    "'" + Provider.serverID + "'," +
                    "'" + player.IP.ToString() + "'," +
                    "'" + player.Ping.ToString() + "'," +
                    Convert.ToInt32(player.IsPro) + "," +
                    Convert.ToInt32(player.IsAdmin) + "," +
                    Convert.ToInt32(player.GodMode) + "," +
                    Convert.ToInt32(player.VanishMode) + "," +
                    Convert.ToInt32(player.IsInVehicle) + "," +
                    "'" + player.Position.ToString() + "'," +
                    "'" + player.Rotation.ToString() + "'," +
                    "'" + player.Player.clothing.skin.ToString() + "'," +
                    player.Player.clothing.hair + "," +
                    player.Player.clothing.face + "," +
                    player.Player.clothing.beard + "," +
                    player.Player.clothing.hat + "," +
                    player.Player.clothing.glasses + "," +
                    player.Player.clothing.mask + "," +
                    Convert.ToInt32(player.Bleeding) + "," +
                    Convert.ToInt32(player.Broken) + "," +
                    player.Health + "," +
                    player.Stamina + "," +
                    player.Hunger + "," +
                    player.Thirst + "," +
                    player.Infection + "," +
                    player.Experience + "," +
                    player.Reputation + "," + 
                    player.GetSkillLevel(UnturnedSkill.Agriculture) + "," +
                    player.GetSkillLevel(UnturnedSkill.Cardio) + "," +
                    player.GetSkillLevel(UnturnedSkill.Cooking) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Crafting) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Dexerity) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Diving) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Engineer) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Exercise) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Fishing) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Healing) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Immunity) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Mechanic) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Outdoors) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Overkill) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Parkour) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Sharpshooter) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Sneakybeaky) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Strength) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Survival) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Toughness) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Vitality) + "," + 
                    player.GetSkillLevel(UnturnedSkill.Warmblooded) + "," +
                    Livemap.Instance.IsPlayerHidden(player) +
                    ")";

                    MySQLCommand.ExecuteNonQuery();
                }
                else
                {
                    // player already exists, update player row
                    MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET " +
                    "character_name = '" + player.CharacterName.ToString() + "'," +
                    "display_name = '" + player.DisplayName.ToString() + "'," +
                    "steam_group_id = '" + player.SteamGroupID.ToString() + "'," +
                    "steam_avatar_medium = '" + player.SteamProfile.AvatarMedium.ToString() + "'," +
                    "server_id = '" + Provider.serverID + "'," +
                    "ip_address = '" + player.IP.ToString() + "'," +
                    "ping = '" + player.Ping.ToString() + "'," +
                    "is_pro = " + Convert.ToInt32(player.IsPro) + "," +
                    "is_admin = " + Convert.ToInt32(player.IsAdmin) + "," +
                    "is_god = " + Convert.ToInt32(player.GodMode) + "," +
                    "is_vanished = " + Convert.ToInt32(player.VanishMode) + "," +
                    "in_vehicle = " + Convert.ToInt32(player.IsInVehicle) + "," +
                    "vehicle_is_driver = " + ReturnVehicleData(player, "vehicle_is_driver") + ", " +
                    "vehicle_instance_id = " + ReturnVehicleData(player, "vehicle_instance_id") + ", " +
                    "vehicle_id = " + ReturnVehicleData(player, "vehicle_id") + ", " +
                    "vehicle_fuel = " + ReturnVehicleData(player, "vehicle_fuel") + ", " +
                    "vehicle_health = " + ReturnVehicleData(player, "vehicle_health") + ", " +
                    "vehicle_headlights_on = " + ReturnVehicleData(player, "vehicle_headlights_on") + ", " +
                    "vehicle_taillights_on = " + ReturnVehicleData(player, "vehicle_taillights_on") + ", " +
                    "vehicle_sirens_on = " + ReturnVehicleData(player, "vehicle_sirens_on") + ", " +
                    "vehicle_speed = " + ReturnVehicleData(player, "vehicle_speed") + ", " +
                    "vehicle_has_battery = " + ReturnVehicleData(player, "vehicle_has_battery") + ", " +
                    "vehicle_battery_charge = " + ReturnVehicleData(player, "vehicle_battery_charge") + ", " +
                    "vehicle_exploded = " + ReturnVehicleData(player, "vehicle_exploded") + ", " +
                    "vehicle_locked = " + ReturnVehicleData(player, "vehicle_locked") + ", " +
                    "position = '" + player.Position.ToString() + "'," +
                    "rotation = '" + player.Rotation.ToString() + "'," +
                    "face = " + player.Player.clothing.face + "," +
                    "hat = " + player.Player.clothing.hat + "," +
                    "glasses = " + player.Player.clothing.glasses + "," +
                    "mask = " + player.Player.clothing.mask + "," +
                    "is_bleeding = " + Convert.ToInt32(player.Bleeding) + "," +
                    "is_broken = " + Convert.ToInt32(player.Broken) + "," +
                    "health = " + player.Health + "," +
                    "stamina = " + player.Stamina + "," +
                    "hunger = " + player.Hunger + "," +
                    "thirst = " + player.Thirst + "," +
                    "infection = " + player.Infection + "," +
                    "experience = " + player.Experience + "," +
                    "reputation = " + player.Reputation + "," +
                    "skill_agriculture = " + player.GetSkillLevel(UnturnedSkill.Agriculture) + "," +
                    "skill_cardio = " + player.GetSkillLevel(UnturnedSkill.Cardio) + "," +
                    "skill_cooking = " + player.GetSkillLevel(UnturnedSkill.Cooking) + "," +
                    "skill_crafting = " + player.GetSkillLevel(UnturnedSkill.Crafting) + "," +
                    "skill_dexerity = " + player.GetSkillLevel(UnturnedSkill.Dexerity) + "," +
                    "skill_diving = " + player.GetSkillLevel(UnturnedSkill.Diving) + "," +
                    "skill_engineer = " + player.GetSkillLevel(UnturnedSkill.Engineer) + "," +
                    "skill_exercise = " + player.GetSkillLevel(UnturnedSkill.Exercise) + "," +
                    "skill_fishing = " + player.GetSkillLevel(UnturnedSkill.Fishing) + "," +
                    "skill_healing = " + player.GetSkillLevel(UnturnedSkill.Healing) + "," +
                    "skill_immunity = " + player.GetSkillLevel(UnturnedSkill.Immunity) + "," +
                    "skill_mechanic = " + player.GetSkillLevel(UnturnedSkill.Mechanic) + "," +
                    "skill_outdoors = " + player.GetSkillLevel(UnturnedSkill.Outdoors) + "," +
                    "skill_overkill = " + player.GetSkillLevel(UnturnedSkill.Overkill) + "," +
                    "skill_parkour = " + player.GetSkillLevel(UnturnedSkill.Parkour) + "," +
                    "skill_sharpshooter = " + player.GetSkillLevel(UnturnedSkill.Sharpshooter) + "," +
                    "skill_sneakybeaky = " + player.GetSkillLevel(UnturnedSkill.Sneakybeaky) + "," +
                    "skill_strength = " + player.GetSkillLevel(UnturnedSkill.Strength) + "," +
                    "skill_survival = " + player.GetSkillLevel(UnturnedSkill.Survival) + "," +
                    "skill_toughness = " + player.GetSkillLevel(UnturnedSkill.Toughness) + "," +
                    "skill_vitality = " + player.GetSkillLevel(UnturnedSkill.Vitality) + "," +
                    "skill_warmblooded = " + player.GetSkillLevel(UnturnedSkill.Warmblooded) + "," +
                    "is_hidden = " + Livemap.Instance.IsPlayerHidden(player) + " " +
                    "WHERE CSteamID = '" + player.CSteamID.ToString() + "'";

                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public int ReturnVehicleData(UnturnedPlayer player, string type)
        {
            Dictionary<string, int> vehicleData = new Dictionary<string, int>(); ;

            if (player.IsInVehicle)
            {
                vehicleData.Add("vehicle_is_driver", Convert.ToInt32(player.CurrentVehicle.isDriver));
                vehicleData.Add("vehicle_instance_id", Convert.ToInt32(player.CurrentVehicle.instanceID));
                vehicleData.Add("vehicle_id", Convert.ToInt32(player.CurrentVehicle.id));
                vehicleData.Add("vehicle_fuel", Convert.ToInt32(player.CurrentVehicle.fuel));
                vehicleData.Add("vehicle_health", Convert.ToInt32(player.CurrentVehicle.health));
                vehicleData.Add("vehicle_headlights_on", Convert.ToInt32(player.CurrentVehicle.headlightsOn));
                vehicleData.Add("vehicle_taillights_on", Convert.ToInt32(player.CurrentVehicle.taillightsOn));
                vehicleData.Add("vehicle_sirens_on", Convert.ToInt32(player.CurrentVehicle.sirensOn));
                vehicleData.Add("vehicle_speed", Convert.ToInt32(player.CurrentVehicle.speed));
                vehicleData.Add("vehicle_has_battery", Convert.ToInt32(player.CurrentVehicle.hasBattery));
                vehicleData.Add("vehicle_battery_charge", Convert.ToInt32(player.CurrentVehicle.batteryCharge));
                vehicleData.Add("vehicle_exploded", Convert.ToInt32(player.CurrentVehicle.isExploded));
                vehicleData.Add("vehicle_locked", Convert.ToInt32(player.CurrentVehicle.isLocked));

                return vehicleData[type];
            }
            else
            {
                vehicleData.Add("vehicle_is_driver", 0);
                vehicleData.Add("vehicle_instance_id", 0);
                vehicleData.Add("vehicle_id", 0);
                vehicleData.Add("vehicle_fuel", 0);
                vehicleData.Add("vehicle_health", 0);
                vehicleData.Add("vehicle_headlights_on", 0);
                vehicleData.Add("vehicle_taillights_on", 0);
                vehicleData.Add("vehicle_sirens_on", 0);
                vehicleData.Add("vehicle_speed", 0);
                vehicleData.Add("vehicle_has_battery", 0);
                vehicleData.Add("vehicle_battery_charge", 0);
                vehicleData.Add("vehicle_exploded", 0);
                vehicleData.Add("vehicle_locked", 0);

                return vehicleData[type];
            }
        }


        /* Event Queries */

        public void OnPlayerConnected(UnturnedPlayer player)
        {
            try
            {
                // insert/update connected player
                RefreshPlayer(player);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void OnPlayerDisconnected(UnturnedPlayer player)
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player disconnect time
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_disconnect = NOW() WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLConnection.Open();

                MySQLCommand.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void OnPlayerDead(UnturnedPlayer player, Vector3 position)
        {
            try
            { 
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update death location and player death status
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_dead_position = '" + player.Position.ToString() + "', is_dead = 1 WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLConnection.Open();

                MySQLCommand.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            try
            { 
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player disconnect time
                //MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET is_dead = 1 WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLConnection.Open();

                MySQLCommand.ExecuteNonQuery();
                MySQLConnection.Close();
                // save kill data
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
            try
            { 
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player dead status
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET is_dead = 0 WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLConnection.Open();

                MySQLCommand.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode)
        {
            // save world chat, but not commands
            if (Livemap.Instance.Configuration.Instance.WorldChatEnabled && chatMode == EChatMode.GLOBAL && !message.StartsWith("/") && !message.StartsWith("@")) {
                try
                {
                    MySqlConnection MySQLConnection = CreateConnection();
                    MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                    // update player experience
                    //MySQLCommand.CommandText = "INSERT INTO " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapChat + " (steam_id,message,server_id) VALUES ('" + player.CSteamID.ToString() + "','" + message + "','" + Provider.serverID + "')";
                    MySQLCommand.CommandText = string.Format("INSERT INTO {0} (steam_id,message,server_id) VALUES ( @steam_id, @message, @server_id);", Livemap.Instance.Configuration.Instance.DatabaseTableLivemapChat);
                    MySQLCommand.Parameters.AddWithValue("@steam_id", player.CSteamID.ToString());
                    MySQLCommand.Parameters.AddWithValue("@message", message);
                    MySQLCommand.Parameters.AddWithValue("@server_id", Provider.serverID);
                    MySQLConnection.Open();

                    MySQLCommand.ExecuteNonQuery();
                    MySQLConnection.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public void OnPlayerUpdateExperience(UnturnedPlayer player, uint experience)
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player experience
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET experience = " + experience + " WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLConnection.Open();

                MySQLCommand.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void Unload()
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player disconnect time
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_disconnect = now()";
                MySQLConnection.Open();

                MySQLCommand.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
