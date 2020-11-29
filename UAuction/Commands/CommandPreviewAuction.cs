using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UAuction.Commands
{
    class CommandTestPreview : IRocketCommand
    {
        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Player;
        public string Name { get; } = "previewauction";
        public string Help { get; } = "";
        public string Syntax { get; } = "";
        public List<string> Aliases { get; } = new List<string>()
        {
            "pauction",
            "pauc",
            "previewauc"
        };
        public List<string> Permissions { get; } = new List<string>()
        {
            "previewauction"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            var currentAuction = Plugin.Instance.AuctionManager.CurrentAuction;
            if(currentAuction == null)
            {
                Plugin.Say(player, "AUCTION_NOT_RUNNING", Color.red);
                return;
            }

            var session = Plugin.Instance.SessionManager.Sessions.FirstOrDefault(c => c.Player.channel.owner.playerID.steamID.m_SteamID == player.CSteamID.m_SteamID);
            session.OpenPreview(currentAuction.Auction);
        }
    }
}
