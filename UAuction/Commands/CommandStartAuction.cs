using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAuction.Auctions;
using UAuction.Serialization;
using UnityEngine;

namespace UAuction.Commands
{
    public sealed class CommandStartAuction : IRocketCommand
    {
        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Player;
        public string Name { get; } = "startauction";
        public string Help { get; } = "";
        public string Syntax { get; } = "";
        public List<string> Aliases { get; }  = new List<string>()
        {
            "startauc"
        };

        public List<string> Permissions { get; } = new List<string>()
        {
            "startauction"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if(command.Length < 1)
            {
                Plugin.Say(player, "INVALID_SYNTAX_START_AUCTION", Color.red);
                return;
            }

            if(!decimal.TryParse(command[0], out var startingBid))
            {
                Plugin.Say(player, "NOT_NUMBER", Color.red, command[0]);
                return;
            }

            if(startingBid < Plugin.Instance.Configuration.Instance.MinimumBid)
            {
                Plugin.Say(player, "BID_TOO_SMALL", Color.red, Plugin.Instance.Configuration.Instance.MinimumBid);
                return;
            }

            if (Plugin.Instance.AuctionManager.HasAnyAuction(player.CSteamID.m_SteamID))
            {
                Plugin.Say("ALREADY_QUEUED", Color.red);
                return;
            }
            //NO need for lock since all operations on AuctionQuehe happens on the main thread
            var auction = new Auction(player.Player, new AuctionItem(), startingBid, TimeSpan.FromSeconds(Plugin.Instance.Configuration.Instance.AuctionDuration));

            var session = Plugin.Instance.SessionManager.Sessions.FirstOrDefault(c => c.Player.channel.owner.playerID.steamID.m_SteamID == player.CSteamID.m_SteamID);
            session.CurrentAuctionCreation = auction;
            session.OpenStartAuctionWindow();
        }
    }
}
