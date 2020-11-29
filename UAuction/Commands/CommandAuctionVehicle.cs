using Rocket.API;
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
    public class CommandAuctionVehicle : IRocketCommand
    {
        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Player;
        public string Name { get; } = "auctionvehicle";
        public string Help { get; } = "";
        public string Syntax { get; } = "";
        public List<string> Aliases { get; } = new List<string>()
        {
            "aucvehicle",
            "aucv"
        };

        public List<string> Permissions { get; } = new List<string>()
        {
            "auctionvehicle"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                Plugin.Say(player, "INVALID_SYNTAX_AUCTION_VEHICLE", Color.red);
                return;
            }

            if(player.CurrentVehicle == null || player.CurrentVehicle.passengers.First().player != player.Player.channel.owner)
            {
                Plugin.Say(player, "NO_VEHICLE", Color.red);
                return;
            }

            if (!decimal.TryParse(command[0], out var startingBid))
            {
                Plugin.Say(player, "NOT_NUMBER", Color.red, command[0]);
                return;
            }

            if (startingBid < Plugin.Instance.Configuration.Instance.MinimumBid)
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
            var auction = new Auction(player.Player, new AuctionItem()
            {
                Vehicle = SerializableVehicle.Create(player.CurrentVehicle)
            }, startingBid, TimeSpan.FromSeconds(Plugin.Instance.Configuration.Instance.AuctionDuration));
            var vehicle = player.CurrentVehicle;
            foreach (var currentPlayer in player.CurrentVehicle.passengers.Where(c => c.player != null))
            {
                vehicle.forceRemovePlayer(out var seat, currentPlayer.player.playerID.steamID, out var point, out var angle);
                VehicleManager.sendExitVehicle(vehicle, seat, point, angle, true);
            }

            if(BarricadeManager.tryGetPlant(vehicle.transform, out byte x, out byte y, out ushort plant, out var region))
            {
                vehicle.trunkItems.items.Clear();
                region.barricades.Clear();
                region.drops.Clear();
            }

            VehicleManager.instance.channel.send("tellVehicleDestroy", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, vehicle.instanceID);

            Plugin.Instance.AuctionManager.AuctionQueue.Add(auction);
            Plugin.Instance.AuctionManager.Tick();

            if (Plugin.Instance.AuctionManager.AuctionQueue.Count > 0) //If it didn't automaticly get started
                Plugin.Say(player, "AUCTION_QUEUED", Color.green, Plugin.Instance.AuctionManager.AuctionQueue.Count,
                    (int)((Plugin.Instance.AuctionManager.LastCompletedAuction.AddSeconds(Plugin.Instance.Configuration.Instance.IntervalInbetween) - DateTime.UtcNow).TotalSeconds));

        }
    }
}
