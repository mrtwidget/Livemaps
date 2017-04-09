using Rocket.Unturned.Player;
using Steamworks;
using I18N.West;
using SDG.Unturned;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using Rocket.API;
using System;

namespace NEXIS.Livemap
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        /* ENSURE DATABASE SCHEMA IS APPLIED */
        public void CheckSchema()
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                // Check if the Table Exists
                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Livemap.Instance.Configuration.Instance.DatabaseTable + "'";
                MySQLConnection.Open();
                object TableExists = MySQLCommand.ExecuteScalar();

                if (TableExists == null)
                {
                    // Create Table
                    MySQLCommand.CommandText = "CREATE TABLE `" + Livemap.Instance.Configuration.Instance.DatabaseTable + "` (`id` INT(11) NOT NULL AUTO_INCREMENT, `steam64ID` VARCHAR(32) NOT NULL, `lastUpdate` TIMESTAMP NOT NULL ON UPDATE CURRENT_TIMESTAMP, PRIMARY KEY (`id`));";
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
    }
}
