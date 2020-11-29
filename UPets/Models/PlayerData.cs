using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.PetsPlugin.Models
{
    public class PlayerData
    {
        public ulong PlayerId { get; set; }
        public List<ushort> Pets { get; set; }
        public ushort lastPetUsed;
        public PlayerData(ulong player, List<ushort> pets, ushort lastPetUsed)
        {
            this.PlayerId = player;
            this.Pets = pets;
            this.lastPetUsed = lastPetUsed;
        }
        public PlayerData() { }
    }
}
