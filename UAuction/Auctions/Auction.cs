using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using UAuction.Utilities;

namespace UAuction.Auctions
{
    public class Auction : IDisposable
    {
        public Auction(Player owner, AuctionItem auctionItem, decimal startingBid, TimeSpan duration)
        {
            this.Owner = new CachedPlayer(owner);
            this.AuctionItem = auctionItem;
            this.StartingBid = startingBid;
            this.Duration = duration;
        }
        public AuctionItem AuctionItem { get; }
        public decimal StartingBid { get; }
        public CachedPlayer Owner { get; }
        public TimeSpan Duration { get; }
        public ICollection<AuctionBid> Bids { get; } = new List<AuctionBid>();


        public decimal GetBidPrice(Player player, decimal amount)
        {
            var bid = Bids.FirstOrDefault(c => c.Player.Id == player.channel.owner.playerID.steamID.m_SteamID);

            if (bid == null)
                return amount;
            return amount - bid.Amount;

        }

        public bool AddBid(Player player, decimal amount)
        {
            if (!IsValidBid(amount, out decimal currentBid))
                return false;

            GetBidPrice(player, amount);

            var bid = Bids.FirstOrDefault(c => c.Player.Id == player.channel.owner.playerID.steamID.m_SteamID);

            if (bid == null)
            {
                Bids.Add(new AuctionBid()
                {
                    Player = new CachedPlayer(player),
                    Amount = amount
                });
            }
            else
            {
                bid.Amount = amount;
            }
            return true;
        }

        public bool IsValidBid(decimal amount, out decimal currentBid)
        {
            currentBid = (Bids.FirstOrDefault()?.Amount ?? StartingBid);
            return amount > currentBid;
        }

        public void GivebackItems()
        {
            Plugin.Instance.AwardManager.RewardPlayer(Owner.Id, AuctionItem);
        }

        public void GivebackBids(params ulong[] exceptions)
        {
            foreach(var bid in Bids)
            {
                if (exceptions.Contains (bid.Player.Id))
                    continue;
                Plugin.Instance.AwardManager.RewardPlayer(bid.Player.Id, bid.Amount);
            }
        }

        public void ResolveAuction(ulong winner, decimal bidAmount)
        {
            Plugin.Instance.AwardManager.RewardPlayer(winner, AuctionItem);
            Plugin.Instance.AwardManager.RewardPlayer(Owner.Id, bidAmount);
        }

        public void Dispose()
        {
            GivebackItems();
            GivebackBids();
        }
    }
}