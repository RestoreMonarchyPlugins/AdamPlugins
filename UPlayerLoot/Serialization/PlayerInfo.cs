using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPlayerLoot.Serialization
{
    public class PlayerInfo
    {
        public ulong Id { get; set; }

        public List<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>();
    }
}
