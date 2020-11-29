using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAuction.UI;
using UnityEngine;

namespace UAuction.Auctions
{
    public class AuctionManager
    {
        public AuctionManager()
        {

        }
        private Tuple<CancellationTokenSource, Task> cancellationToken;
        
        public ICollection<Auction> AuctionQueue { get; } = new List<Auction>();

        public RunningAuction CurrentAuction { get; private set; } = default;
        public DateTime AuctionComplete { get; private set; }

        public DateTime LastCompletedAuction { get; private set; } = DateTime.UtcNow;

        public Task Start(CancellationToken cancellationToken)
        {
            var task = Task.Run(async () =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    TaskDispatcher.QueueOnMainThread(() =>
                    {
                        Tick();
                    });

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
            this.cancellationToken = new Tuple<CancellationTokenSource, Task>(CancellationTokenSource.CreateLinkedTokenSource(cancellationToken), task);
            return Task.CompletedTask;
        }

        internal void Tick()
        {
            if (CheckForNewAuctionStarting())
                return;
            if (CurrentAuction != null)
            {
                CurrentAuction.CheckForAnnouncments();
                if (CurrentAuction.CheckForFinish())
                {
                    CurrentAuction = null;
                    Plugin.Instance.UIManager.DestroyUi();
                    LastCompletedAuction = DateTime.UtcNow;
                }
            }
        }

        public async Task Stop()
        {
            if (cancellationToken.Item2.IsCompleted)
                return;
            cancellationToken.Item1.Cancel();
            await Task.WhenAll(cancellationToken.Item2);
        }

        private bool CheckForNewAuctionStarting()
        {
            if (CurrentAuction == null
                && (DateTime.UtcNow - LastCompletedAuction).TotalSeconds > Plugin.Instance.Configuration.Instance.IntervalInbetween
                && AuctionQueue.Count > 0)
            {
                var found = AuctionQueue.First();
                AuctionQueue.Remove(found);
                StartAuction(found);
                return true;
            }
            return false;
        }

        public bool StartAuction(Auction auction)
        {
            if (CurrentAuction != null)
                return false;
            Plugin.Say("AUCTION_STARTING", Color.green, auction.Owner.CharacterName, auction.StartingBid);
            this.CurrentAuction = new RunningAuction(auction);
            foreach (var session in Plugin.Instance.SessionManager.Sessions.Where(c => c.IsUiEnabled))
            {
                Plugin.Instance.UIManager.SpawnUi(session.Player.channel.owner.playerID.steamID);
            }

            return true;
        }


        public bool HasAnyAuction(ulong id)
        {
            return (CurrentAuction != null && CurrentAuction.Auction.Owner.Id == id) || AuctionQueue.Any(c => c.Owner.Id == id) ;
        }
    }
}
