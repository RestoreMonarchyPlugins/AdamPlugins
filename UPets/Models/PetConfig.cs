using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.PetsPlugin.Models
{
    public class PetConfig
    {
        public string Name { get; set; }
        public ushort Id { get; set; }
        public decimal Cost { get; set; }
        public string RequiredPermission { get; set; }
        public double MaxAliveTime { get; set; }

        public PetConfig(string name, ushort id, decimal cost, string requiredPermission, double maxAliveTime)
        {
            Name = name;
            Id = id;
            Cost = cost;
            RequiredPermission = requiredPermission;
            MaxAliveTime = maxAliveTime;
        }

        public PetConfig() { }
    }
}
