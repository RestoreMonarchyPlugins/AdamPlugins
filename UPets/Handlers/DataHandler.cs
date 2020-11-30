﻿using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Adam.PetsPlugin
{
    public static class DataHandler
    {
        private static Database _database;


        public static Database database
        {
            get
            {
                if (_database == null && PetsPlugin.Instance.Configuration.Instance.UseMySQL)
                    _database = new Database();
                return _database;
            }
            set
            {
                if (value == null)
                    return;
                _database = value;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static PlayerData getPlayerD(ulong steamId)
        {

            Database.updateMySQLDetailsIsCorrect();
            if (!Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) { Rocket.Core.Logging.Logger.LogError("PetsPlugin -> MySQL details incorrect | Can't connect to database!"); return null; }
            if (Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) //use mysql
            {
                return database.getPlayerData(steamId);
            }
            else
            {
                return PetsPlugin.Instance.Configuration.Instance.PlayerData.Find(c => (c.player == steamId));
            }
        }

        public static bool setLatestPet(ulong steamId, ushort petId = 0)
        {
            Database.updateMySQLDetailsIsCorrect();

            if (!Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) { Rocket.Core.Logging.Logger.LogError("PetsPlugin -> MySQL details incorrect | Can't connect to database!"); return false; }

            if (Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) //use mysql
            {
                return database.setLatestPet(steamId, petId);
            }
            else //Not using mysql
            {
                var item = PetsPlugin.Instance.Configuration.Instance.PlayerData.Find(c => (c.player == steamId));
                if (item == null)
                {
                    PetsPlugin.Instance.Configuration.Instance.PlayerData.Add(new PlayerData(steamId, new List<ushort>(), petId));
                }
                else item.lastPetUsed = petId;
                PetsPlugin.Instance.Configuration.Save();
                return true;
            }
        }

        public static bool addPetToList(ulong steamId, ushort petId)
        {
            Database.updateMySQLDetailsIsCorrect();

            if (!Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) { Rocket.Core.Logging.Logger.LogError("PetsPlugin -> MySQL details incorrect | Can't connect to database!"); return false; }

            if (Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) //use mysql
            {
                return database.addPet(steamId, petId);
            }
            else //Not using mysql
            {
                var item = PetsPlugin.Instance.Configuration.Instance.PlayerData.Find(c => (c.player == steamId));
                if (item == null)
                {
                    PetsPlugin.Instance.Configuration.Instance.PlayerData.Add(new PlayerData(steamId, new List<ushort>() { petId }, 0));
                }
                else
                {
                    if (item.pets.Contains(petId)) return false;
                    item.pets.Add(petId);
                    PetsPlugin.Instance.Configuration.Save();
                    return true;
                }
                PetsPlugin.Instance.Configuration.Save();
                return true;
            }
        }

        public static bool removePetToList(ulong steamId, ushort petId)
        {
            //Database.updateMySQLDetailsIsCorrect();

            if (!Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) { Rocket.Core.Logging.Logger.LogError("PetsPlugin -> MySQL details incorrect | Can't connect to database!"); return false; }

            if (Database.MySQLDetailsCorrect && PetsPlugin.Instance.Configuration.Instance.UseMySQL) //use mysql
            {
                return database.removePet(steamId, petId);
            }
            else //Not using mysql
            {
                var item = PetsPlugin.Instance.Configuration.Instance.PlayerData.Find(c => (c.player == steamId));
                if (item == null)
                {
                    PetsPlugin.Instance.Configuration.Instance.PlayerData.Add(new PlayerData(steamId, new List<ushort>(), 0));
                }
                else
                {
                    bool output = item.pets.Remove(petId);
                    PetsPlugin.Instance.Configuration.Save();
                    return output;
                }
                PetsPlugin.Instance.Configuration.Save();
                return true;
            }
        }

    }

    public class Database
    {
        private Dictionary<CSteamID, PlayerData> _cache = new Dictionary<CSteamID, PlayerData>();

        private static bool _MySQLDetailsCorrect = false;

        public static bool MySQLDetailsCorrect
        {
            get
            {
                return _MySQLDetailsCorrect;
            }
        }

        public static void updateMySQLDetailsIsCorrect()
        {
            if (!PetsPlugin.Instance.Configuration.Instance.UseMySQL)
                return;
            MySqlConnection connection = null;
            try
            {
                if (PetsPlugin.Instance.Configuration.Instance.DatabasePort == 0) PetsPlugin.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};Connect Timeout=5;", PetsPlugin.Instance.Configuration.Instance.DatabaseAddress, PetsPlugin.Instance.Configuration.Instance.DatabaseName, PetsPlugin.Instance.Configuration.Instance.DatabaseUsername, PetsPlugin.Instance.Configuration.Instance.DatabasePassword, PetsPlugin.Instance.Configuration.Instance.DatabasePort))
                {
                    
                };

            }
            catch(Exception e)
            {
                _MySQLDetailsCorrect = false;
                Console.WriteLine(e);
                return;
            }
            if (connection == null)
                _MySQLDetailsCorrect = false;
            else _MySQLDetailsCorrect = true;
        }

        public Database()
        {
            Console.WriteLine(PetsPlugin.Instance == null);

            Console.WriteLine(PetsPlugin.Instance.Configuration == null);
            Console.WriteLine(PetsPlugin.Instance.Configuration.Instance == null);
            if (!PetsPlugin.Instance.Configuration.Instance.UseMySQL)
                return;

            updateMySQLDetailsIsCorrect();
            if (!MySQLDetailsCorrect) return;
            CheckSchema();
            U.Events.OnPlayerDisconnected += Disconnected;
        }

        private void Disconnected(UnturnedPlayer player)
        {
            _cache.Remove(player.CSteamID);
        }

        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (PetsPlugin.Instance.Configuration.Instance.DatabasePort == 0) PetsPlugin.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", PetsPlugin.Instance.Configuration.Instance.DatabaseAddress, PetsPlugin.Instance.Configuration.Instance.DatabaseName, PetsPlugin.Instance.Configuration.Instance.DatabaseUsername, PetsPlugin.Instance.Configuration.Instance.DatabasePassword, PetsPlugin.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public PlayerData getPlayerData(ulong playerId)
        {
            if (_cache.TryGetValue((CSteamID)playerId, out var result2))
                return result2;
            ulong steamid = playerId;
            List<ushort> pets = new List<ushort>();
            ushort latestPet = 0;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `latestPet` from `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersTableName + "` where `steamId` = '" + playerId.ToString() + "'";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) ushort.TryParse(result.ToString(), out latestPet);
                command.CommandText = "SELECT `pet` FROM `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersDataTableName + "` WHERE `steamId` + '" + playerId.ToString() + "'";
                var _reader = command.ExecuteReader();
                while (_reader.Read())
                {
                    string item = Convert.ToString(_reader["pet"]);
                    ushort i = 0;
                    bool isu = ushort.TryParse(item, out i);
                    if (!isu) continue;
                    pets.Add(i);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            PlayerData output = new PlayerData(steamid, pets, latestPet);
            _cache.Add((CSteamID)playerId, output);
            return output;
        }
       
        public bool setLatestPet(ulong playerId, ushort petId)
        {
            bool outPut = true;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "update `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersTableName + "` set `latestPet` = '" + petId + "' where `steamId` = '" + playerId.ToString() + "'";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result == null)
                {
                    command.CommandText = "insert ignore into `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersTableName + "` (steamId,latestPet) values('" + playerId.ToString() + "','" + petId.ToString() + "')";
                    result = command.ExecuteScalar();
                    outPut = true;

                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                outPut = false;
            }
            return outPut;
        }

        public bool addPet(ulong steamId, ushort petId)
        {
            bool output = true;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "select `pet` from `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersDataTableName + "` where `steamId` = '" + steamId + "' and `pet` = '" + petId.ToString() + "'";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    output = false;
                }
                else
                {
                    command.CommandText = "insert ignore into `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersDataTableName + "` (steamId,pet) values('" + steamId.ToString() + "','" + petId.ToString() + "')";
                    command.ExecuteScalar();
                    output = true;
                }
                connection.Close();
            }
            catch
            {
                output = false;
            }
            return output;
        }

        public bool removePet(ulong steamId, ushort petId)
        {
            bool output = true;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "select `pet` from `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersDataTableName + "` where `steamId` = '" + steamId.ToString() + "' and `pet` = '" + petId.ToString() + "'";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result == null)
                {
                    output = false;
                }
                else
                {
                    command.CommandText = "DELETE `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersDataTableName + "` WHERE `steamId` = '" + steamId.ToString() + "' AND `pet` = '" + petId.ToString() + "'";
                    command.ExecuteScalar();
                    output = true;
                }
                connection.Close();
            }
            catch
            {
                output = false;
            }
            return output;
        }

        internal void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersTableName + "` (`steamId` varchar(32) NOT NULL DEFAULT '0',`latestPet` varchar(10) NOT NULL DEFAULT '0',PRIMARY KEY (`steamId`)) ";
                    command.ExecuteNonQuery();
                }
                command.CommandText = "show tables like '" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersDataTableName + "'";
                test = command.ExecuteScalar();
                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + PetsPlugin.Instance.Configuration.Instance.DatabasePlayersDataTableName + "` (`steamId` varchar(32) NOT NULL DEFAULT '0',`pet` varchar(10) NOT NULL DEFAULT '0') ";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}

