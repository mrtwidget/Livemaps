using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using SDG.Unturned;
using MySql.Data.MySqlClient;
using System;
using UnityEngine;

namespace NEXIS.Livemap
{
    public class DatabaseManager
    {
        MySqlConnection MySQLConnection = null;

        /**
         * Construct Class
         * 
         * This function includes required database DLL and checks the schema
         */
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            MySQLConnection = CreateConnection();
            MySQLConnection.Open();
            CheckSchema();
        }

        /**
         * Check Schema / Create Database Tables
         * 
         * This function checks if the required database tables already exist 
         * in the database and creates
         * them if they do not already exist.
         */
        public void CheckSchema()
        {
            /* `livemap_server` */
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + "'";
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
                    "refresh_interval INT(10) NOT NULL DEFAULT 10," +
                    "last_refresh TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                    "PRIMARY KEY(server_id));";

                    MySQLCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            /* `livemap_data` */
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + "'";
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
                    "last_disconnect TIMESTAMP DEFAULT NULL," +
                    "PRIMARY KEY(CSteamID));";

                    MySQLCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            
            /* `livemap_chat` */
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapChat + "'";
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
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         * MySQL Database Connection
         * 
         * This function creates a connection to a mysql database, for use with
         * queries required in this file.
         */
        private MySqlConnection CreateConnection()
        {
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

        /**
         * Refresh Server Data
         * 
         * This function updates all server data in the livemap+server table
         * with the most current information.
         */
        public void UpdateServerData()
        {
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if server exists in table
                MySQLCommand.CommandText = "SELECT * FROM " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + " WHERE server_id = '" + Provider.serverID + "'";
                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // server does not exist in database, create it
                    MySQLCommand.CommandText = string.Format("INSERT INTO {0} (server_id,server_name,app_version,map,online_players,max_players,is_pvp,is_gold,is_pro,has_cheats,hide_admins,cycle_time,cycle_length,full_moon,uptime,packets_received,packets_sent,port,mode,refresh_interval) VALUES (@server_id,@server_name,@app_version,@map,@online_players,@max_players,@is_pvp,@is_gold,@is_pro,@has_cheats,@hide_admins,@cycle_time,@cycle_length,@full_moon,@uptime,@packets_received,@packets_sent,@port,@mode,@refresh_interval)", Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer);
                }
                else
                {
                    // server already exists, update server info
                    MySQLCommand.CommandText = string.Format("UPDATE {0} SET server_name=@server_name,app_version=@app_version,map=@map,online_players=@online_players,max_players=@max_players,is_pvp=@is_pvp,is_gold=@is_gold,is_pro=@is_pro,has_cheats=@has_cheats,hide_admins=@hide_admins,cycle_time=@cycle_time,cycle_length=@cycle_length,full_moon=@full_moon,uptime=@uptime,packets_received=@packets_received,packets_sent=@packets_sent,port=@port,mode=@mode,refresh_interval=@refresh_interval WHERE server_id=@server_id", Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer);
                }

                MySQLCommand.Parameters.AddWithValue("@server_id", Provider.serverID);
                MySQLCommand.Parameters.AddWithValue("@server_name", Provider.serverName);
                MySQLCommand.Parameters.AddWithValue("@app_version", Provider.APP_VERSION);
                MySQLCommand.Parameters.AddWithValue("@map", Provider.map);
                MySQLCommand.Parameters.AddWithValue("@online_players", Provider.clients.Count);
                MySQLCommand.Parameters.AddWithValue("@max_players", Provider.maxPlayers);
                MySQLCommand.Parameters.AddWithValue("@is_pvp", Convert.ToInt32(Provider.isPvP));
                MySQLCommand.Parameters.AddWithValue("@is_gold", Convert.ToInt32(Provider.isGold));
                MySQLCommand.Parameters.AddWithValue("@is_pro", Convert.ToInt32(Provider.isPro));
                MySQLCommand.Parameters.AddWithValue("@has_cheats", Convert.ToInt32(Provider.hasCheats));
                MySQLCommand.Parameters.AddWithValue("@hide_admins", Convert.ToInt32(Provider.hideAdmins));
                MySQLCommand.Parameters.AddWithValue("@cycle_time", LightingManager.time);
                MySQLCommand.Parameters.AddWithValue("@cycle_length", LightingManager.cycle);
                MySQLCommand.Parameters.AddWithValue("@full_moon", Convert.ToInt32(LightingManager.isFullMoon));
                MySQLCommand.Parameters.AddWithValue("@uptime", Time.time);
                MySQLCommand.Parameters.AddWithValue("@packets_received", Provider.packetsReceived);
                MySQLCommand.Parameters.AddWithValue("@packets_sent", Provider.packetsSent);
                MySQLCommand.Parameters.AddWithValue("@port", Provider.port);
                MySQLCommand.Parameters.AddWithValue("@mode", Provider.mode.ToString());
                MySQLCommand.Parameters.AddWithValue("@refresh_interval", Livemap.Instance.Configuration.Instance.LivemapRefreshInterval);

                MySQLCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         * Update Player Database Row
         * 
         * This function updates the database with current player information.
         * @param {UnturnedPlayer} player Player data
         */
        public void UpdatePlayer(UnturnedPlayer player)
        {
            MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

            // check if player exists in table
            MySQLCommand.CommandText = "SELECT * FROM " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
            object result = MySQLCommand.ExecuteScalar();

            if (result == null)
            {
                // insert - player does not exist in database
                MySQLCommand.CommandText = string.Format("INSERT INTO {0} (CSteamID,character_name,display_name,steam_group_id,steam_avatar_medium,server_id,ip_address,ping,is_pro,is_admin,is_god,is_vanished,in_vehicle,position,rotation,skin_color,hair,face,beard,hat,glasses,mask,is_bleeding,is_broken,health,stamina,hunger,thirst,infection,experience,reputation,skill_agriculture,skill_cardio,skill_cooking,skill_crafting,skill_dexerity,skill_diving,skill_engineer,skill_exercise,skill_fishing,skill_healing,skill_immunity,skill_mechanic,skill_outdoors,skill_overkill,skill_parkour,skill_sharpshooter,skill_sneakybeaky,skill_strength,skill_survival,skill_toughness,skill_vitality,skill_warmblooded,is_hidden) VALUES (@CSteamID,@character_name,@display_name,@steam_group_id,@steam_avatar_medium,@server_id,@ip_address,@ping,@is_pro,@is_admin,@is_god,@is_vanished,@in_vehicle,@position,@rotation,@skin_color,@hair,@face,@beard,@hat,@glasses,mask,@is_bleeding,@is_broken,@health,@stamina,@hunger,@thirst,@infection,@experience,@reputation,@skill_agriculture,@skill_cardio,@skill_cooking,@skill_crafting,@skill_dexerity,@skill_diving,@skill_engineer,@skill_exercise,@skill_fishing,@skill_healing,@skill_immunity,@skill_mechanic,@skill_outdoors,@skill_overkill,@skill_parkour,@skill_sharpshooter,@skill_sneakybeaky,@skill_strength,@skill_survival,@skill_toughness,@skill_vitality,@skill_warmblooded,@is_hidden)", Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData);
            }
            else
            {
                // update - player exists in database
                MySQLCommand.CommandText = string.Format("UPDATE {0} SET character_name=@character_name,display_name=@display_name,steam_group_id=@steam_group_id,steam_avatar_medium=@steam_avatar_medium,server_id=@server_id,ip_address=@ip_address,ping=@ping,is_pro=@is_pro,is_admin=@is_admin,is_god=@is_god,is_vanished=@is_vanished,in_vehicle=@in_vehicle,vehicle_is_driver=@vehicle_is_driver,vehicle_instance_id=@vehicle_instance_id,vehicle_id=@vehicle_id,vehicle_fuel=@vehicle_fuel,vehicle_health=@vehicle_health,vehicle_headlights_on=@vehicle_headlights_on,vehicle_taillights_on=@vehicle_taillights_on,vehicle_sirens_on=@vehicle_sirens_on,vehicle_speed=@vehicle_speed,vehicle_has_battery=@vehicle_has_battery,vehicle_battery_charge=@vehicle_battery_charge,vehicle_exploded=@vehicle_exploded,vehicle_locked=@vehicle_locked,position=@position,rotation=@rotation,skin_color=@skin_color,hair=@hair,face=@face,beard=@beard,hat=@hat,glasses=@glasses,mask=@mask,is_bleeding=@is_bleeding,is_broken=@is_broken,health=@health,stamina=@stamina,hunger=@hunger,thirst=@thirst,infection=@infection,experience=@experience,reputation=@reputation,skill_agriculture=@skill_agriculture,skill_cardio=@skill_cardio,skill_cooking=@skill_cooking,skill_crafting=@skill_crafting,skill_dexerity=@skill_dexerity,skill_diving=@skill_diving,skill_engineer=@skill_engineer,skill_exercise=@skill_exercise,skill_fishing=@skill_fishing,skill_healing=@skill_healing,skill_immunity=@skill_immunity,skill_mechanic=@skill_mechanic,skill_outdoors=@skill_outdoors,skill_overkill=@skill_overkill,skill_parkour=@skill_parkour,skill_sharpshooter=@skill_sharpshooter,skill_sneakybeaky=@skill_sneakybeaky,skill_strength=@skill_strength,skill_survival=@skill_survival,skill_toughness=@skill_toughness,skill_vitality=@skill_vitality,skill_warmblooded=@skill_warmblooded,is_hidden=@is_hidden WHERE CSteamID=@CSteamID", Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData);
            }

            MySQLCommand.Parameters.AddWithValue("@CSteamID", player.CSteamID.ToString());
            MySQLCommand.Parameters.AddWithValue("@character_name", player.CharacterName);
            MySQLCommand.Parameters.AddWithValue("@display_name", player.DisplayName);
            MySQLCommand.Parameters.AddWithValue("@steam_group_id", (player.SteamGroupID.ToString() != null ? player.SteamGroupID.ToString() : "0"));
            MySQLCommand.Parameters.AddWithValue("@steam_avatar_medium", (player.SteamProfile.AvatarMedium != null ? player.SteamProfile.AvatarMedium.ToString() : Livemap.Instance.Configuration.Instance.PlayerDefaultSteamAvatar));
            MySQLCommand.Parameters.AddWithValue("@server_id", Provider.serverID);
            MySQLCommand.Parameters.AddWithValue("@ip_address", player.IP.ToString());
            MySQLCommand.Parameters.AddWithValue("@ping", player.Ping.ToString());
            MySQLCommand.Parameters.AddWithValue("@is_pro", Convert.ToInt32(player.IsPro));
            MySQLCommand.Parameters.AddWithValue("@is_admin", Convert.ToInt32(player.IsAdmin));
            MySQLCommand.Parameters.AddWithValue("@is_god", Convert.ToInt32(player.GodMode));
            MySQLCommand.Parameters.AddWithValue("@is_vanished", Convert.ToInt32(player.VanishMode));
            MySQLCommand.Parameters.AddWithValue("@in_vehicle", Convert.ToInt32(player.IsInVehicle));
            MySQLCommand.Parameters.AddWithValue("@vehicle_is_driver", ReturnVehicleData(player, "vehicle_is_driver"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_instance_id", ReturnVehicleData(player, "vehicle_instance_id"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_id", ReturnVehicleData(player, "vehicle_id"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_fuel", ReturnVehicleData(player, "vehicle_fuel"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_health", ReturnVehicleData(player, "vehicle_health"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_headlights_on", ReturnVehicleData(player, "vehicle_headlights_on"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_taillights_on", ReturnVehicleData(player, "vehicle_taillights_on"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_sirens_on", ReturnVehicleData(player, "vehicle_sirens_on"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_speed", ReturnVehicleData(player, "vehicle_speed"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_has_battery", ReturnVehicleData(player, "vehicle_has_battery"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_battery_charge", ReturnVehicleData(player, "vehicle_battery_charge"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_exploded", ReturnVehicleData(player, "vehicle_exploded"));
            MySQLCommand.Parameters.AddWithValue("@vehicle_locked", ReturnVehicleData(player, "vehicle_locked"));
            MySQLCommand.Parameters.AddWithValue("@position", player.Position.ToString());
            MySQLCommand.Parameters.AddWithValue("@rotation", player.Rotation.ToString());
            MySQLCommand.Parameters.AddWithValue("@skin_color", ColorTypeConverter.ToRGBHex(player.Player.clothing.skin));
            MySQLCommand.Parameters.AddWithValue("@hair", player.Player.clothing.hair);
            MySQLCommand.Parameters.AddWithValue("@face", player.Player.clothing.face);
            MySQLCommand.Parameters.AddWithValue("@beard", player.Player.clothing.beard);
            MySQLCommand.Parameters.AddWithValue("@hat", player.Player.clothing.hat);
            MySQLCommand.Parameters.AddWithValue("@glasses", player.Player.clothing.glasses);
            MySQLCommand.Parameters.AddWithValue("@mask", player.Player.clothing.mask);
            MySQLCommand.Parameters.AddWithValue("@is_bleeding", Convert.ToInt32(player.Bleeding));
            MySQLCommand.Parameters.AddWithValue("@is_broken", Convert.ToInt32(player.Broken));
            MySQLCommand.Parameters.AddWithValue("@health", player.Health);
            MySQLCommand.Parameters.AddWithValue("@stamina", player.Stamina);
            MySQLCommand.Parameters.AddWithValue("@hunger", player.Hunger);
            MySQLCommand.Parameters.AddWithValue("@thirst", player.Thirst);
            MySQLCommand.Parameters.AddWithValue("@infection", player.Infection);
            MySQLCommand.Parameters.AddWithValue("@experience", player.Experience);
            MySQLCommand.Parameters.AddWithValue("@reputation", player.Reputation);
            MySQLCommand.Parameters.AddWithValue("@skill_agriculture", player.GetSkillLevel(UnturnedSkill.Agriculture));
            MySQLCommand.Parameters.AddWithValue("@skill_cardio", player.GetSkillLevel(UnturnedSkill.Cardio));
            MySQLCommand.Parameters.AddWithValue("@skill_cooking", player.GetSkillLevel(UnturnedSkill.Cooking));
            MySQLCommand.Parameters.AddWithValue("@skill_crafting", player.GetSkillLevel(UnturnedSkill.Crafting));
            MySQLCommand.Parameters.AddWithValue("@skill_dexerity", player.GetSkillLevel(UnturnedSkill.Dexerity));
            MySQLCommand.Parameters.AddWithValue("@skill_diving", player.GetSkillLevel(UnturnedSkill.Diving));
            MySQLCommand.Parameters.AddWithValue("@skill_engineer", player.GetSkillLevel(UnturnedSkill.Engineer));
            MySQLCommand.Parameters.AddWithValue("@skill_exercise", player.GetSkillLevel(UnturnedSkill.Exercise));
            MySQLCommand.Parameters.AddWithValue("@skill_fishing", player.GetSkillLevel(UnturnedSkill.Fishing));
            MySQLCommand.Parameters.AddWithValue("@skill_healing", player.GetSkillLevel(UnturnedSkill.Healing));
            MySQLCommand.Parameters.AddWithValue("@skill_immunity", player.GetSkillLevel(UnturnedSkill.Immunity));
            MySQLCommand.Parameters.AddWithValue("@skill_mechanic", player.GetSkillLevel(UnturnedSkill.Mechanic));
            MySQLCommand.Parameters.AddWithValue("@skill_outdoors", player.GetSkillLevel(UnturnedSkill.Outdoors));
            MySQLCommand.Parameters.AddWithValue("@skill_overkill", player.GetSkillLevel(UnturnedSkill.Overkill));
            MySQLCommand.Parameters.AddWithValue("@skill_parkour", player.GetSkillLevel(UnturnedSkill.Parkour));
            MySQLCommand.Parameters.AddWithValue("@skill_sharpshooter", player.GetSkillLevel(UnturnedSkill.Sharpshooter));
            MySQLCommand.Parameters.AddWithValue("@skill_sneakybeaky", player.GetSkillLevel(UnturnedSkill.Sneakybeaky));
            MySQLCommand.Parameters.AddWithValue("@skill_strength", player.GetSkillLevel(UnturnedSkill.Strength));
            MySQLCommand.Parameters.AddWithValue("@skill_survival", player.GetSkillLevel(UnturnedSkill.Survival));
            MySQLCommand.Parameters.AddWithValue("@skill_toughness", player.GetSkillLevel(UnturnedSkill.Toughness));
            MySQLCommand.Parameters.AddWithValue("@skill_vitality", player.GetSkillLevel(UnturnedSkill.Vitality));
            MySQLCommand.Parameters.AddWithValue("@skill_warmblooded", player.GetSkillLevel(UnturnedSkill.Warmblooded));
            MySQLCommand.Parameters.AddWithValue("@is_hidden", Livemap.Instance.IsPlayerHidden(player));

            MySQLCommand.ExecuteNonQuery();
        }

        public static class ColorTypeConverter
        {
            public static string ToRGBHex(Color c)
            {
                return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
            }

            private static byte ToByte(float f)
            {
                f = Mathf.Clamp01(f);
                return (byte)(f * 255);
            }
        }

        /**
         * Refresh Player Data
         * 
         * This function collects aplayer's current stats and updates the 
         * database to reflect this change.
         */
        public void UpdateAllPlayers()
        {
            try
            {
                // loop through each connected player
                foreach (SteamPlayer plr in Provider.clients)
                {
                    if (plr == null) continue;
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(plr);

                    // update player data
                    UpdatePlayer(player);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         * Return Player Vehicle Data
         * 
         * This function checks if a player is in a vehicle, and collects data
         * from the vehicle if one exists.
         * @param {UnturnedPlayer} player Player data
         * @param {string} prop Vehicle property name
         */
        public int ReturnVehicleData(UnturnedPlayer player, string prop)
        {
            int data = 0;

            if (player.IsInVehicle)
            {
                switch (prop)
                {
                    case "vehicle_is_driver":
                        data = Convert.ToInt32(player.CurrentVehicle.isDriver);
                        break;
                    case "vehicle_instance_id":
                        data = Convert.ToInt32(player.CurrentVehicle.instanceID);
                        break;
                    case "vehicle_id":
                        data = Convert.ToInt32(player.CurrentVehicle.id);
                        break;
                    case "vehicle_fuel":
                        data = Convert.ToInt32(player.CurrentVehicle.fuel);
                        break;
                    case "vehicle_health":
                        data = Convert.ToInt32(player.CurrentVehicle.health);
                        break;
                    case "vehicle_headlights_on":
                        data = Convert.ToInt32(player.CurrentVehicle.headlightsOn);
                        break;
                    case "vehicle_taillights_on":
                        data = Convert.ToInt32(player.CurrentVehicle.taillightsOn);
                        break;
                    case "vehicle_sirens_on":
                        data = Convert.ToInt32(player.CurrentVehicle.sirensOn);
                        break;
                    case "vehicle_speed":
                        data = Convert.ToInt32(player.CurrentVehicle.speed);
                        break;
                    case "vehicle_has_battery":
                        data = Convert.ToInt32(player.CurrentVehicle.hasBattery);
                        break;
                    case "vehicle_battery_charge":
                        data = Convert.ToInt32(player.CurrentVehicle.batteryCharge);
                        break;
                    case "vehicle_exploded":
                        data = Convert.ToInt32(player.CurrentVehicle.isExploded);
                        break;
                    case "vehicle_locked":
                        data = Convert.ToInt32(player.CurrentVehicle.isLocked);
                        break;
                }

                return data;
            }
            else
            {
                return 0;
            }
        }

        /* EVENTS */

        public void OnPlayerConnected(UnturnedPlayer player)
        {
            try
            {
                // update / insert connected player row
                UpdatePlayer(player);

                // update player connection time
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_connect = NULL WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLCommand.ExecuteScalar();
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
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player disconnect time
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_disconnect = NULL WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLCommand.ExecuteNonQuery();
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
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update death location and player death status
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_dead_position = '" + player.Position.ToString() + "', is_dead = 1 WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLCommand.ExecuteNonQuery();
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
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player dead status
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET is_dead = 0 WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLCommand.ExecuteNonQuery();
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
                    MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                    // add world chat message to database
                    MySQLCommand.CommandText = string.Format("INSERT INTO {0} (steam_id,message,server_id) VALUES ( @steam_id, @message, @server_id);", Livemap.Instance.Configuration.Instance.DatabaseTableLivemapChat);
                    MySQLCommand.Parameters.AddWithValue("@steam_id", player.CSteamID.ToString());
                    MySQLCommand.Parameters.AddWithValue("@message", message);
                    MySQLCommand.Parameters.AddWithValue("@server_id", Provider.serverID);

                    MySQLCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public void CleanUp()
        {
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player disconnect timestamps 
                // where connection timestamp is greater than disconnect timestamp (currently connected)
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_disconnect = NULL WHERE last_connect > last_disconnect";
                MySQLCommand.ExecuteNonQuery();

                // close mysql connection
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
