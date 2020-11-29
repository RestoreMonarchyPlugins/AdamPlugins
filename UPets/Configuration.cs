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

        public List<PlayerD> PlayerData { get; set; }

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

            PlayerData = new List<PlayerD>();
        }
    }

    public class PetAsset
    {
        public string name;
        public ushort id;
        public decimal cost;
        public string requiredPermission;
        public bool ifHasPermissionGetForFree;
        public double maxAliveTime;

        public PetAsset(string name, ushort id, decimal cost, string requiredPermission, bool ifHasPermissionGetForFree, double maxAliveTime)
        {

            this.name = name;
            this.id = id;
            this.cost = cost;
            this.requiredPermission = requiredPermission;
            this.ifHasPermissionGetForFree = ifHasPermissionGetForFree;
            this.maxAliveTime = maxAliveTime;
        }
        public PetAsset() { }

        public bool HasPet(ulong player)
        {
            RocketPlayer p = new RocketPlayer(player.ToString());
            if (ifHasPermissionGetForFree && p.HasPermission(requiredPermission))
                return true;
            if (DataHandler.getPlayerD(player) != null && DataHandler.getPlayerD(player).pets.Contains(id)) return true;
            return false;
        }
    }

    public class PlayerD
    {
        public ulong player;
        [XmlArrayItem("pet")]
        public List<ushort> pets;
        public ushort lastPetUsed;
        public PlayerD(ulong player, List<ushort> pets, ushort lastPetUsed)
        {
            this.player = player;
            this.pets = pets;
            this.lastPetUsed = lastPetUsed;
        }
        public PlayerD() { }
    }
}
