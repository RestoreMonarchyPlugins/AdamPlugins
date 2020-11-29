using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UAuction.Commands
{
    public sealed class CommandBid : IRocketCommand
    {
        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Player;
        public string Name { get; } = "bid";
        public string Help { get; } = "";
        public string Syntax { get; } = "";
        public List<string> Aliases { get; } = new List<string>();
        public List<string> Permissions { get; } = new List<string>()
        {
            "bid"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            var auction = Plugin.Instance.AuctionManager.CurrentAuction;
            if(auction == null)
            {
                Plugin.Say(player, "AUCTION_NOT_RUNNING", Color.red);
                return;
            }

            if(auction.Auction.Owner.Id == player.CSteamID.m_SteamID)
            {
                Plugin.Say(player, "CANT_BID_SELF", Color.red);
                return;
            }

            if(!decimal.TryParse(command[0], out var amount) && amount > 0)
            {
                Plugin.Say(player, "NOT_NUMBER", Color.red, command[0]);

                return;
            }

            
            decimal pay = auction.Auction.GetBidPrice(player.Player, amount);
            if(Uconomy.Instance.Database.GetBalance(player.Id)  < pay)
            {
                Plugin.Say(player, "CANT_AFFORD", Color.red, pay);
                return;
            }
            //Check balance
            if(!auction.Auction.IsValidBid(amount, out decimal currentBid) || !auction.Auction.AddBid(player.Player, amount))
            {
                Plugin.Say(player, "UNDER_BID", Color.red, currentBid);
                return;
            }
            
            Plugin.Say("BID", Color.green, player.CharacterName, amount);
            Uconomy.Instance.Database.IncreaseBalance(player.Id, pay * -1);

            var span = auction.FinishDate - DateTime.UtcNow;
            var span2 = TimeSpan.FromSeconds(Plugin.Instance.Configuration.Instance.MinimumTimeAfterBid);
            if (span < span2)
            {
                auction.FinishDate.Add(span2 - span);
            }
        }
    }
}
