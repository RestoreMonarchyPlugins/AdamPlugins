using Rocket.API;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace UAuction
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int IntervalInbetween { get; set; }
        public int AuctionDuration { get; set; }
        public int MinimumTimeAfterBid { get; set; }

        public decimal MinimumBid { get; set; }
        public byte ContainerWidth { get; set; }
        public byte ContainerHeight { get; set; }
        public ushort UiEffect { get; set; }

        public List<AnnouncmentItem> Announcments { get; set; } = new List<AnnouncmentItem>();

        public string IconUrl { get; set; }

        public void LoadDefaults()
        {
            IntervalInbetween = 30;
            AuctionDuration = 40;
            MinimumTimeAfterBid = 10;

            ContainerWidth = 10;
            ContainerHeight = 15;
            MinimumBid = 500;
            IconUrl = "https://i.imgur.com/cEKbdkt.png";
            UiEffect = 50010;

            Announcments.Add(new AnnouncmentItem()
            {
                Color = "green",
                Message = "Theres only 30 seconds left on the auction!",
                Second = 30
            });
        }
    }

    public class AnnouncmentItem
    {
        public double Second { get; set; }
        public string Message { get; set; }
        public string Color { get; set; }
    }
}