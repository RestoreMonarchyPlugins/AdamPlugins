using Rocket.Unturned;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAuction.Sessions
{
    public class SessionManager
    {
        public SessionManager()
        {
            U.Events.OnPlayerConnected += Connected;
            U.Events.OnPlayerDisconnected += Disconnected;
        }

        public ICollection<PlayerSession> Sessions { get; } = new List<PlayerSession>();

        private void Disconnected(UnturnedPlayer player)
        {
            var session = Sessions.FirstOrDefault(c => c.Player.channel.owner.playerID.steamID == player.CSteamID);
            Sessions.Remove(session);
        }

        private void Connected(UnturnedPlayer player)
        {
            Sessions.Add(new PlayerSession(player.Player));
        }
    }
}
