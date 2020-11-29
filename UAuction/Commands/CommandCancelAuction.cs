using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace UAuction.Commands
{
    public class CommandCancelAuction : IRocketCommand
    {
        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Player;
        public string Name { get; } = "cancelauction";
        public string Help { get; } = "";
        public string Syntax { get; } = "";
        public List<string> Aliases { get; } = new List<string>()
        {
            "cancelauc",
            "cauc"
        };
        public List<string> Permissions { get; } = new List<string>()
        {
            "cancelauction"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if((Plugin.Instance.AuctionManager.CurrentAuction?.Auction?.Owner?.Id ?? 0) == player.CSteamID.m_SteamID)
            {
                Plugin.Say(player, "CANCEL_AUCTION_RUNNING", Color.red);
                return;
            }
            var found = Plugin.Instance.AuctionManager.AuctionQueue.FirstOrDefault(c => c.Owner.Id == player.CSteamID.m_SteamID);
            if (found == null)
            {
                Plugin.Say(player, "CANCEL_NOT_IN_QUEUE", Color.red);
                return;
            }
            found.Dispose();
            Plugin.Instance.AuctionManager.AuctionQueue.Remove(found);
            Plugin.Say(player, "CANCELLED", Color.green);

        }
    }
}
