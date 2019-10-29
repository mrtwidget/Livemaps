using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using SDG.Unturned;
using MySql.Data.MySqlClient;
using System;

namespace NEXIS.Livemap
{
    public class Database
    {
        public MySqlConnection MySQLConnection = null;

        public Database()
        {
            new I18N.West.CP1250();
            MySQLConnection = CreateConnection();
            MySQLConnection.Open();
            CheckSchema();
        }

        public void CheckSchema()
        {
            /* `livemap_server` */
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if the table exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" +
                                           Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + "'";
                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    // table doesn't exist, create it
                    MySQLCommand.CommandText =
                        "CREATE TABLE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer +
                        "(server_id VARCHAR(32) NOT NULL," +
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
                        "mode VARCHAR(32) NULL," +
                        "refresh_interval INT(10) NOT NULL DEFAULT 10," +
                        "last_refresh TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                        "PRIMARY KEY(server_id));";

                    MySQLCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Logger.LogException(ex); }

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
                    "steam_avatar_medium VARCHAR(255) NULL," +
                    "ip_address VARCHAR(255) NULL," +
                    "ping DECIMAL(8,6) NULL," +
                    "is_admin TINYINT(1) NULL," +
                    "is_god TINYINT(1) NULL," +
                    "is_vanished TINYINT(1) NULL," +
                    "in_vehicle TINYINT(1) NULL," +
                    "position VARCHAR(32) NULL," +
                    "is_dead TINYINT(1) NOT NULL DEFAULT 0," +
                    "face INT(8) NULL," +
                    "is_bleeding TINYINT(1) NULL," +
                    "is_broken TINYINT(1) NULL," +
                    "health INT(8) NULL," +
                    "stamina INT(8) NULL," +
                    "hunger INT(8) NULL," +
                    "thirst INT(8) NULL," +
                    "infection INT(8) NULL," +
                    "experience INT(8) NULL," +
                    "reputation INT(8) NULL," +
                    "last_connect TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
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
                    "name VARCHAR(32) NULL," +
                    "steam_id VARCHAR(32) NULL," +
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

        private MySqlConnection CreateConnection()
        {
            try
            {
                if (Livemap.Instance.Configuration.Instance.DatabasePort == 0)
                    Livemap.Instance.Configuration.Instance.DatabasePort = 3306;

                MySQLConnection = new MySqlConnection(
                    $"SERVER={Livemap.Instance.Configuration.Instance.DatabaseHost};DATABASE={Livemap.Instance.Configuration.Instance.DatabaseName};UID={Livemap.Instance.Configuration.Instance.DatabaseUser};PASSWORD={Livemap.Instance.Configuration.Instance.DatabasePass};PORT={Livemap.Instance.Configuration.Instance.DatabasePort};");
            }
            catch (MySqlException ex) { Logger.LogException(ex); }

            return MySQLConnection;
        }

        public void UpdateServerData()
        {
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if server exists in table
                MySQLCommand.CommandText = "SELECT * FROM " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer + " WHERE server_id = '" + Provider.serverID + "'";
                object result = MySQLCommand.ExecuteScalar();

                MySQLCommand.CommandText = string.Format(
                    result == null
                        ? "INSERT INTO {0} (server_id,server_name,app_version,map,online_players,max_players,is_pvp,is_gold,has_cheats,hide_admins,cycle_time,cycle_length,full_moon,mode,refresh_interval) VALUES (@server_id,@server_name,@app_version,@map,@online_players,@max_players,@is_pvp,@is_gold,@has_cheats,@hide_admins,@cycle_time,@cycle_length,@full_moon,@mode,@refresh_interval)"
                        : "UPDATE {0} SET server_name=@server_name,app_version=@app_version,map=@map,online_players=@online_players,max_players=@max_players,is_pvp=@is_pvp,is_gold=@is_gold,has_cheats=@has_cheats,hide_admins=@hide_admins,cycle_time=@cycle_time,cycle_length=@cycle_length,full_moon=@full_moon,mode=@mode,refresh_interval=@refresh_interval WHERE server_id=@server_id",

                    Livemap.Instance.Configuration.Instance.DatabaseTableLivemapServer);

                #region Parameters

                MySQLCommand.Parameters.AddWithValue("@server_id", Livemap.Instance.Server.ID);
                MySQLCommand.Parameters.AddWithValue("@server_name", Livemap.Instance.Server.Name);
                MySQLCommand.Parameters.AddWithValue("@app_version", Livemap.Instance.Server.Version);
                MySQLCommand.Parameters.AddWithValue("@map", Livemap.Instance.Server.Map);
                MySQLCommand.Parameters.AddWithValue("@online_players", Livemap.Instance.Server.PlayersOnline);
                MySQLCommand.Parameters.AddWithValue("@max_players", Livemap.Instance.Server.MaxPlayers);
                MySQLCommand.Parameters.AddWithValue("@is_pvp", Convert.ToInt32(Livemap.Instance.Server.PVP));
                MySQLCommand.Parameters.AddWithValue("@is_gold", Convert.ToInt32(Livemap.Instance.Server.Gold));
                MySQLCommand.Parameters.AddWithValue("@has_cheats", Convert.ToInt32(Livemap.Instance.Server.HasCheats));
                MySQLCommand.Parameters.AddWithValue("@hide_admins", Convert.ToInt32(Livemap.Instance.Server.HideAdmins));
                MySQLCommand.Parameters.AddWithValue("@cycle_time", Livemap.Instance.Server.Time);
                MySQLCommand.Parameters.AddWithValue("@cycle_length", Livemap.Instance.Server.Cycle);
                MySQLCommand.Parameters.AddWithValue("@full_moon", Convert.ToInt32(Livemap.Instance.Server.FullMoon));
                MySQLCommand.Parameters.AddWithValue("@mode", Livemap.Instance.Server.Mode);
                MySQLCommand.Parameters.AddWithValue("@refresh_interval", Livemap.Instance.Configuration.Instance.LivemapRefreshInterval);

                #endregion

                MySQLCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }

        public void UpdatePlayer(UnturnedPlayer player)
        {
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // check if player exists in table
                MySQLCommand.CommandText = "SELECT * FROM " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                object result = MySQLCommand.ExecuteScalar();

                MySQLCommand.CommandText = string.Format(
                    result == null
                        ? "INSERT INTO {0} (CSteamID,character_name,steam_avatar_medium,ip_address,ping,is_admin,is_god,is_vanished,in_vehicle,position,face,is_bleeding,is_broken,health,stamina,hunger,thirst,infection,experience,reputation) VALUES (@CSteamID,@character_name,@steam_avatar_medium,@ip_address,@ping,@is_admin,@is_god,@is_vanished,@in_vehicle,@position,@face,@is_bleeding,@is_broken,@health,@stamina,@hunger,@thirst,@infection,@experience,@reputation)"
                        : "UPDATE {0} SET character_name=@character_name,steam_avatar_medium=@steam_avatar_medium,ip_address=@ip_address,ping=@ping,is_admin=@is_admin,is_god=@is_god,is_vanished=@is_vanished,in_vehicle=@in_vehicle,position=@position,face=@face,is_bleeding=@is_bleeding,is_broken=@is_broken,health=@health,stamina=@stamina,hunger=@hunger,thirst=@thirst,infection=@infection,experience=@experience,reputation=@reputation WHERE CSteamID=@CSteamID",

                    Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData);

                #region Parameters

                MySQLCommand.Parameters.AddWithValue("@CSteamID", Livemap.Instance.Nodes[player.CSteamID].SteamID);
                MySQLCommand.Parameters.AddWithValue("@character_name", Livemap.Instance.Nodes[player.CSteamID].CharacterName);
                MySQLCommand.Parameters.AddWithValue("@steam_avatar_medium", Livemap.Instance.Nodes[player.CSteamID].Avatar);
                MySQLCommand.Parameters.AddWithValue("@ip_address", Livemap.Instance.Nodes[player.CSteamID].IP);
                MySQLCommand.Parameters.AddWithValue("@ping", Livemap.Instance.Nodes[player.CSteamID].Ping.ToString());
                MySQLCommand.Parameters.AddWithValue("@is_admin", Convert.ToInt32(Livemap.Instance.Nodes[player.CSteamID].IsAdmin));
                MySQLCommand.Parameters.AddWithValue("@is_god", Convert.ToInt32(Livemap.Instance.Nodes[player.CSteamID].GodMode));
                MySQLCommand.Parameters.AddWithValue("@is_vanished", Convert.ToInt32(Livemap.Instance.Nodes[player.CSteamID].VanishMode));
                MySQLCommand.Parameters.AddWithValue("@in_vehicle", Convert.ToInt32(Livemap.Instance.Nodes[player.CSteamID].IsInVehicle));
                MySQLCommand.Parameters.AddWithValue("@position", Livemap.Instance.Nodes[player.CSteamID].Position.ToString());
                MySQLCommand.Parameters.AddWithValue("@face", Livemap.Instance.Nodes[player.CSteamID].Face);
                MySQLCommand.Parameters.AddWithValue("@is_bleeding", Convert.ToInt32(Livemap.Instance.Nodes[player.CSteamID].Bleeding));
                MySQLCommand.Parameters.AddWithValue("@is_broken", Convert.ToInt32(Livemap.Instance.Nodes[player.CSteamID].Broken));
                MySQLCommand.Parameters.AddWithValue("@health", Livemap.Instance.Nodes[player.CSteamID].Health);
                MySQLCommand.Parameters.AddWithValue("@stamina", Livemap.Instance.Nodes[player.CSteamID].Stamina);
                MySQLCommand.Parameters.AddWithValue("@hunger", Livemap.Instance.Nodes[player.CSteamID].Hunger);
                MySQLCommand.Parameters.AddWithValue("@thirst", Livemap.Instance.Nodes[player.CSteamID].Thirst);
                MySQLCommand.Parameters.AddWithValue("@infection", Livemap.Instance.Nodes[player.CSteamID].Infection);
                MySQLCommand.Parameters.AddWithValue("@experience", Livemap.Instance.Nodes[player.CSteamID].Experience);
                MySQLCommand.Parameters.AddWithValue("@reputation", Livemap.Instance.Nodes[player.CSteamID].Reputation);

                #endregion

                MySQLCommand.ExecuteNonQuery();

            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }

        public void UpdateAllPlayers()
        {
            try
            {
                // loop through each connected player
                for (int i = 0; i < Provider.clients.Count; i++)
                {
                    SteamPlayer plr = Provider.clients[i];

                    if (plr == null) continue;

                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(plr);

                    // update player data
                    UpdatePlayer(player);
                }
            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }

        public void OnPlayerConnected(UnturnedPlayer player)
        {
            try
            {
                // update / insert connected player row
                UpdatePlayer(player);

                // update player connection time
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET last_connect = NULL WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }

        public void OnPlayerDead(UnturnedPlayer player)
        {
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update death location and player death status
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET is_dead = 1 WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }

        public void OnPlayerRevive(UnturnedPlayer player)
        {
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // update player dead status
                MySQLCommand.CommandText = "UPDATE " + Livemap.Instance.Configuration.Instance.DatabaseTableLivemapData + " SET is_dead = 0 WHERE CSteamID = '" + player.CSteamID.ToString() + "'";
                MySQLCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }

        public void OnPlayerChatted(UnturnedPlayer player, string message)
        {
            try
            {
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // add world chat message to database
                MySQLCommand.CommandText =
                    $"INSERT INTO {Livemap.Instance.Configuration.Instance.DatabaseTableLivemapChat} (steam_id,name,message) VALUES ( @steam_id, @name, @message );";

                MySQLCommand.Parameters.AddWithValue("@steam_id", player.CSteamID.ToString());
                MySQLCommand.Parameters.AddWithValue("@name", player.CharacterName);
                MySQLCommand.Parameters.AddWithValue("@message", message);

                MySQLCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }

        public void CleanUp()
        {
            try
            {
                // close mysql connection
                MySQLConnection.Close();
            }
            catch (MySqlException ex) { Logger.LogException(ex); }
        }
    }
}