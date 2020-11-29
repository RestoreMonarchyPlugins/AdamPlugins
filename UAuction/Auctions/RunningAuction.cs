using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UAuction.Auctions
{
    public class RunningAuction
    {
        public RunningAuction(Auction auction)
        {
            this.Auction = auction ?? throw new ArgumentNullException(nameof(auction));
            this.FinishDate = DateTime.UtcNow.Add(Auction.Duration);
            AnnouncmentsLeft = Plugin.Instance.Configuration.Instance.Announcments
                .OrderByDescending(c => c.Second)
                .ToList(); //Copy it.
        }
        public Auction Auction { get; set; }
        public DateTime FinishDate { get; set; }
        private List<AnnouncmentItem> AnnouncmentsLeft { get; }

        public void CheckForAnnouncments()
        {
            TimeSpan timeLeft = FinishDate - DateTime.UtcNow;

            if (timeLeft < TimeSpan.Zero)
                return;
            var found = AnnouncmentsLeft.FirstOrDefault(c => c.Second > timeLeft.TotalSeconds);
            if (found == null)
                return;
            AnnouncmentsLeft.Remove(found);
            Plugin.Say(found.Message, UnturnedChat.GetColorFromName(found.Color, Color.green));
        }

        public bool CheckForFinish()
        {
            bool result = DateTime.UtcNow > FinishDate;
            if (result)
            {
                var winner = Auction.Bids.LastOrDefault();
                if (winner == null)
                {
                    Plugin.Say("AUCTION_FINISHED_NO_WINNER", Color.green);
                    Auction.GivebackItems();
                    return result;
                }

                for (int i = 1; i < Auction.Bids.Count; i++)
                {
                    var bid = Auction.Bids.ElementAt(i);
                    Plugin.Instance.AwardManager.RewardPlayer(bid.Player.Id, bid.Amount);
                    if (!bid.Player.IsOnline)
                        continue;
                    UnturnedPlayer player = UnturnedPlayer.FromPlayer(bid.Player.Player);
                    Plugin.Say(player, "LOST_AUCTION", Color.green, bid.Amount);
                }

               Plugin.Say("AUCTION_FINISHED", Color.green, winner.Player.CharacterName, winner.Amount);
                Auction.ResolveAuction(winner.Player.Id, winner.Amount);
            }
            return result;
        }
    }
}
