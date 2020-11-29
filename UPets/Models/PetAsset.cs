using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.PetsPlugin.Models
{
    public class PetAsset
    {
        public string Name { get; set; }
        public ushort Id { get; set; }
        public decimal Cost { get; set; }
        public string RequiredPermission { get; set; }
        public bool IfHasPermissionGetForFree { get; set; }
        public double MaxAliveTime { get; set; }

        public PetAsset(string name, ushort id, decimal cost, string requiredPermission, bool ifHasPermissionGetForFree, double maxAliveTime)
        {

            Name = name;
            Id = id;
            Cost = cost;
            RequiredPermission = requiredPermission;
            IfHasPermissionGetForFree = ifHasPermissionGetForFree;
            MaxAliveTime = maxAliveTime;
        }
        public PetAsset() { }

        public bool HasPet(ulong player)
        {
            RocketPlayer p = new RocketPlayer(player.ToString());
            if (IfHasPermissionGetForFree && p.HasPermission(RequiredPermission))
                return true;
            if (DataHandler.getPlayerD(player) != null && DataHandler.getPlayerD(player).pets.Contains(Id)) return true;
            return false;
        }
    }
}
