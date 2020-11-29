using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Adam.PetsPlugin
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string LicenseKey;
        public List<PetAsset> Pets = new List<PetAsset>();
        public bool useMySQL;
        public float maxDistanceBetweenPetAndOwner;
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabasePlayersTableName;
        public string DatabasePlayersDataTableName;
        public int DatabasePort;


        public string Branch { get; set; } = "default";
        public string BranchPassword { get; set; }


        public List<PlayerD> PlayerData = new List<PlayerD>();

        public void LoadDefaults()
        {
            
            LicenseKey = Guid.Empty.ToString();
            useMySQL = false;
            maxDistanceBetweenPetAndOwner = 50;
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabasePlayersTableName = "generalPD";
            DatabasePlayersDataTableName = "listPD";
            DatabasePort = 3306;
            Pets.Add(new PetAsset("cow", 6, 100, "cow", false, 600));
            Pets.Add(new PetAsset("bear", 5, 250, "bear", true, 800));
            Pets.Add(new PetAsset("wolf", 3, 150, "wolf", false, 1600));
            Pets.Add(new PetAsset("reindeer", 7, 500, "reindeer", false, 1600));
            Pets.Add(new PetAsset("pig", 4, 150, "pig", false, 1600));
            Pets.Add(new PetAsset("deer", 1, 150, "deer", false, 1600));

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

        public bool hasPet(ulong player)
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
