using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UAuction.Auctions;
using UAuction.Serialization;
using UnityEngine;

namespace UAuction.Sessions
{
    public class PlayerSession
    {
        private const uint InstanceIdUsing = 765765;
        private const ushort PlantIdUsing = 35345;
        public PlayerSession(Player player)
        {
            Player = player;
        }

        private Items startAuctionItems;
        private Auction auctionParameter;

        public Auction CurrentAuctionCreation
        {
            get => auctionParameter;
            set
            {
                auctionParameter = value;
                if(auctionParameter == null)
                {
                    startAuctionItems = null;
                    return;
                }

                startAuctionItems = new Items(PlayerInventory.STORAGE);

                startAuctionItems.resize(Plugin.Instance.Configuration.Instance.ContainerWidth, Plugin.Instance.Configuration.Instance.ContainerHeight);
                foreach(var item in auctionParameter.AuctionItem.Items)
                {
                    var position = item.Position.Value;
                    startAuctionItems.addItem(position.X, position.Y, position.Rot, item.ToItem());
                }
            }
        }

        public Player Player { get; }
        public bool IsUiEnabled { get; set; } = true;

        public void OpenPreview(Auction auction)
        {
            if (auction == null)
                throw new ArgumentNullException(nameof(auction));

            if (auction.AuctionItem.Items.Count > 0)
            {
                OpenItemPreview(auction);
            }
            if(auction.AuctionItem.Vehicle != null)
            {
                OpenVehiclePreview(auction);
            }

        }

        private void OpenVehiclePreview(Auction auction)
        {
            var asset = (VehicleAsset)Assets.find(EAssetType.VEHICLE, auction.AuctionItem.Vehicle.VehicleId);
            Plugin.Say(UnturnedPlayer.FromPlayer(Player), "AUCTION_INFO_VEHICLE", Color.green, asset.vehicleName, asset.id);
        }

        private void OpenItemPreview(Auction auction)
        {
            var newInventory = new Items(PlayerInventory.STORAGE);
            newInventory.resize(Plugin.Instance.Configuration.Instance.ContainerWidth, Plugin.Instance.Configuration.Instance.ContainerHeight);
            foreach (var item in auction.AuctionItem.Items)
            {
                var position = item.Position.Value;
                newInventory.addItem(position.X, position.Y, position.Rot, item.ToItem());
            }

            Player.inventory.items[PlayerInventory.STORAGE] = new Items(PlayerInventory.STORAGE);
            Player.inventory.channel.openWrite();
            Player.inventory.channel.write(false);
            Player.inventory.channel.write(newInventory.width, newInventory.height, newInventory.getItemCount());
            for (byte index = 0; (int)index < newInventory.getItemCount(); ++index)
            {
                ItemJar jar = newInventory.getItem(index);
                Player.inventory.channel.write((object)jar.x, jar.y, (object)jar.rot, (object)jar.item.id, (object)jar.item.amount, (object)jar.item.quality, (object)jar.item.state);
            }
            Player.inventory.channel.closeWrite("tellStoraging", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_CHUNK_BUFFER);


        }

        public void OpenStartAuctionWindow()
        {
            if (auctionParameter == null)
                throw new Exception();

            Player.inventory.updateItems(startAuctionItems.page, startAuctionItems);
            Player.inventory.sendStorage();
        }

        internal void OnGesture(EPlayerGesture gesture)
        {
            if (gesture != EPlayerGesture.INVENTORY_STOP)
                return;

            if(auctionParameter != null)
                FinalizeAuctionCreation();            
        }

        public void FinalizeAuctionCreation()
        {
            var player = UnturnedPlayer.FromPlayer(Player);
            try
            {
                auctionParameter.AuctionItem.Items = startAuctionItems.items.Select(c => SerializableItem.CreateSerializableItem(PlayerInventory.STORAGE, c)).ToList();
                if (auctionParameter.AuctionItem.Items.Count == 0)
                {
                    Plugin.Say(player, "NO_ITEMS", Color.red, Plugin.Instance.AuctionManager.AuctionQueue.Count);
                    return;
                }
                Plugin.Instance.AuctionManager.AuctionQueue.Add(auctionParameter);
                Plugin.Instance.AuctionManager.Tick();
                if (Plugin.Instance.AuctionManager.AuctionQueue.Count > 0) //If it didn't automaticly get started
                    Plugin.Say(player, "AUCTION_QUEUED", Color.green, Plugin.Instance.AuctionManager.AuctionQueue.Count,
                        (int)((Plugin.Instance.AuctionManager.LastCompletedAuction.AddSeconds(Plugin.Instance.Configuration.Instance.IntervalInbetween) - DateTime.UtcNow).TotalSeconds));

            }
            finally
            {
                startAuctionItems = null;
                auctionParameter = null;
            }
        }
    }
}