using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using SDG.Unturned;
using I18N.West;
using MySql.Data.MySqlClient;
using Rocket.API;
using System;
using Steamworks;
using UnityEngine;

namespace NEXIS.Livemap
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        public void CheckSchema()
        {
            /* `livemap_servers` */
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServers + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // table doesn't exist, create it
                    MySQLCommand.CommandText = "CREATE TABLE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServers +
                        " ( id INT(10) NOT NULL AUTO_INCREMENT," +
                            "server_id VARCHAR(32) NULL," +
                            "server_name VARCHAR(50) NULL," +
                            "app_version VARCHAR(32) NULL," +
                            "map VARCHAR(32) NULL," +
                            "online_players INT(8) NULL," +
                            "max_players INT(8) NULL," +
                            "is_pvp TINYINT(1) NULL," +
                            "is_gold TINYINT(1) NULL," +
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
                            "PRIMARY KEY(id) );";

                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            /* `livemap_data` */
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
                        " ( CSteamID VARCHAR(32) NOT NULL," +
                            "character_name VARCHAR(100) NULL," +
                            "display_name VARCHAR(100) NULL," +
                            "steam_group_id VARCHAR(32) NULL," +
                            "steam_avatar_medium VARCHAR(255) NULL," +
                            "steam_headline VARCHAR(255) NULL," +
                            "ip_address VARCHAR(255) NULL," +
                            "ping FLOAT(7,8) NULL," +
                            "is_pro TINYINT(1) NULL," +
                            "is_admin TINYINT(1) NULL," +
                            "is_god TINYINT(1) NULL," +
                            "is_vanished TINYINT(1) NULL," +
                            "in_vehicle TINYINT(1) NULL," +
                            "vehicle_enabled TINYINT(1) NULL," +
                            "vehicle_is_driver TINYINT(1) NULL," +
                            "vehicle_instance_id INT(8) NULL," +
                            "vehicle_id INT(8) NULL," +
                            "vehicle_fuel INT(8) NULL," +
                            "vehicle_health INT(8) NULL," +
                            "vehicle_headlights_on TINYINT(1) NULL," +
                            "vehicle_taillights_on TINYINT(1) NULL," +
                            "vehicle_sirens_on TINYINT(1) NULL," +
                            "vehicle_speed VARCHAR(32) NULL," +
                            "vehicle_has_battery TINYINT(1) NULL," +
                            "vehicle_battery_charge INT(8) NULL," +
                            "vehicle_exploded TINYINT(1) NULL," +
                            "vehicle_locked TINYINT(1) NULL," +
                            "vehicle_owner VARCHAR(32) NULL," +
                            "position VARCHAR(32) NULL," +
                            "rotation FLOAT(7,4) NULL," +
                            "is_dead TINYINT(1) NULL," +
                            "skin_color VARCHAR(32) NULL," +
                            "hair INT(8) NULL," +
                            "face INT(8) NULL," +
                            "beard INT(8) NULL," +
                            "visual_hat INT(8) NULL," +
                            "visual_glasses INT(8) NULL," +
                            "visual_mask INT(8) NULL," +
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
                            "is_hidden TINYINT(1) NULL," +
                            "last_refresh TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                            "PRIMARY KEY(CSteamID)) ;";
                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /* CONNECT TO MYSQL DATABASE */
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


        /* Event Queries */

        public void OnPlayerConnected(UnturnedPlayer player)
        {
            // add/update player row
        }

        public void OnPlayerDisconnected(UnturnedPlayer player)
        {
            // add/update player row
        }

        public void OnPlayerDead(UnturnedPlayer player, Vector3 position)
        {
            // save player corpse location
        }

        public void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            // update player death status
            // save kill data
        }

        public void OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
            // update player death status
        }

        public void OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode)
        {
            // save world chat
        }

        public void OnPlayerUpdateExperience(UnturnedPlayer player, uint experience)
        {
            // update database
        }
    }
}
