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
    public class Configuration : IRocketPluginConfiguration
    {
        public List<PetAsset> Pets { get; set; }
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
            DatabasePlayersTableName = "generalPD";
            DatabasePlayersDataTableName = "listPD";
            DatabasePort = 3306;
            Pets = new List<PetAsset>() 
            {
                new PetAsset("cow", 6, 100, "cow", false, 600),
                new PetAsset("bear", 5, 250, "bear", true, 800),
                new PetAsset("wolf", 3, 150, "wolf", false, 1600),
                new PetAsset("reindeer", 7, 500, "reindeer", false, 1600),
                new PetAsset("pig", 4, 150, "pig", false, 1600),
                new PetAsset("deer", 1, 150, "deer", false, 1600)
            };
        }
    }
}
