using Adam.PetsPlugin.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace Adam.PetsPlugin
{
    public class PetsConfiguration : IRocketPluginConfiguration
    {
        public List<PetConfig> Pets { get; set; }
        public bool UseMySQL { get; set; }
        public float MaxDistanceBetweenPetAndOwner { get; set; }
        public string DatabaseAddress { get; set; }
        public string DatabaseUsername { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseName { get; set; }
        public string DatabasePlayersTableName { get; set; }
        public string DatabasePlayersDataTableName { get; set; }
        public int DatabasePort { get; set; }

        public void LoadDefaults()
        {
            UseMySQL = false;
            MaxDistanceBetweenPetAndOwner = 50;
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabasePlayersTableName = "Players";
            DatabasePlayersDataTableName = "PlayersData";
            DatabasePort = 3306;
            Pets = new List<PetConfig>() 
            {
                new PetConfig("cow", 6, 100, "cow", 600),
                new PetConfig("bear", 5, 250, "bear", 800),
                new PetConfig("wolf", 3, 150, "wolf", 1600),
                new PetConfig("reindeer", 7, 500, "reindeer", 1600),
                new PetConfig("pig", 4, 150, "pig", 1600),
                new PetConfig("deer", 1, 150, "deer", 1600)
            };
        }
    }
}
