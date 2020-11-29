using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAuction.UI
{
    public class UIManager
    {
        private const short Key = 7458;
        public UIManager()
        {

        }

        public void RefreshUiText()
        {

            var auction = Plugin.Instance.AuctionManager.CurrentAuction;
            if (auction == null)
                return;

            foreach(var session in Plugin.Instance.SessionManager.Sessions.Where(c => c.IsUiEnabled))
            {
                var bid = auction.Auction.Bids.FirstOrDefault()?.Amount ?? auction.Auction.StartingBid;
                var timeLeft = auction.FinishDate - DateTime.UtcNow;
                EffectManager.sendUIEffectText(Key, session.Player.channel.owner.playerID.steamID, true, "Owner", auction.Auction.Owner.CharacterName);
                EffectManager.sendUIEffectText(Key, session.Player.channel.owner.playerID.steamID, true, "Current", $"{bid.ToString()}$");
                EffectManager.sendUIEffectText(Key, session.Player.channel.owner.playerID.steamID, true, "Info", ((int)timeLeft.TotalSeconds).ToString());
            }
        }

        public void SpawnUi(CSteamID steamID)
        {
            var auction = Plugin.Instance.AuctionManager.CurrentAuction;

            if (auction == null)
                return;
            var bid = auction.Auction.Bids.FirstOrDefault()?.Amount ?? auction.Auction.StartingBid;
            var timeLeft = auction.FinishDate - DateTime.UtcNow;

            EffectManager.sendUIEffect(Plugin.Instance.Configuration.Instance.UiEffect, Key, steamID, true,
                auction.Auction.Owner.CharacterName,
                $"{bid.ToString()}$",
                ((int)timeLeft.TotalSeconds).ToString());

        }

        public void DestroyUi()
        {

            EffectManager.instance.channel.send("tellEffectClearByID", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, Plugin.Instance.Configuration.Instance.UiEffect);

        }
    }
}
