using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAuction.Utilities
{
    public class CachedPlayer
    {
        public CachedPlayer(Player player)
        {
            this.Player = player;
            this.Id = player.channel.owner.playerID.steamID.m_SteamID;
            this.cachedCharacterName = player.channel.owner.playerID.characterName;
        }

        public ulong Id { get; }
        public Player Player { get; }
        public bool IsOnline => Player == null;

        private string cachedCharacterName;
        public string CharacterName
        {
            get
            {
                if (IsOnline)
                    return Player.channel.owner.playerID.characterName;
                return cachedCharacterName;
            }
        }
    }
}
