﻿using HarmonyLib;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAuction.Auctions;
using UAuction.Awards;
using UAuction.Sessions;
using UAuction.UI;
using UnityEngine;

namespace UAuction
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance { get; private set; }
        public AuctionManager AuctionManager { get; } = new AuctionManager();
        public AwardManager AwardManager { get; } = new AwardManager();
        public SessionManager SessionManager { get; } = new SessionManager();
        public UIManager UIManager { get; private set; }

        public const string HarmonyInstanceId = "de.uauction";
        private Harmony HarmonyInstance { get; set; }

        protected override void Load()
        {
            Instance = this;
            HarmonyInstance = new Harmony(HarmonyInstanceId);            
            Level.onLevelLoaded += LevelLoaded;
            Provider.onServerShutdown += Shutdown;
            HarmonyInstance.PatchAll(Assembly);

            UIManager = gameObject.AddComponent<UIManager>();

            Rocket.Core.Logging.Logger.Log($"Made by AdamAdam, maintained by Restore Monarchy Plugins", ConsoleColor.Yellow);
            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            Destroy(UIManager);
            HarmonyInstance.UnpatchAll(HarmonyInstanceId);
            AuctionManager.Stop();
            Rocket.Core.Logging.Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        private void LevelLoaded(int level)
        {
            AwardManager.Load();
            AuctionManager.Start();
        }

        private void Shutdown()
        {
            AuctionManager.CurrentAuction?.Auction?.Dispose();
            foreach(var auction in AuctionManager.AuctionQueue)
            {
                auction.Dispose();
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "UI_TOGGLED", "You've toggled the auction UI to {0}." },
            { "INVALID_SYNTAX_AUCTION_VEHICLE", "Invalid syntax! Usage: /auctionvehicle <starting bid>" },
            { "NO_VEHICLE", "You're currently not sitting in a vehicle's driverseat!" },
            { "AUCTION_INFO_VEHICLE", "Vehicle {0}({1}) is being auctioned away!" },
            { "NO_ITEMS", "No items were left in the auction items box." },
            { "CANT_BID_SELF", "You can't bid on your own auction!" },
            { "BID_TOO_SMALL", "{0} is the minimum bid!" },
            { "NOT_NUMBER", "{0} is not a number!" },
            { "CANCEL_NOT_IN_QUEUE", "You don't have any queued auctions!" },
            { "CANCEL_AUCTION_RUNNING", "You can't cancel a running auction!" },
            { "AUCTION_QUEUED", "Your auction is in queue position {0} with another {1} seconds until the next auction starts!" },
            { "CANCELLED", "You cancelled your queued auction!" },
            { "ALREADY_QUEUED", "You already have an auction queued up or running! You can cancel by typing /auctioncancel" },
            { "CANT_AFFORD", "You can't afford to bid another {0}$!" },
            { "UNDER_BID", "Your below the current bid which is {0}$!" },
            { "INVALID_SYNTAX_START_AUCTION", "Invalid syntax! Usage: /startauction <starting bid>"  },
            { "AUCTION_FINISHED_NO_WINNER", "The auction had no bids." },
            { "AUCTION_FINISHED", "Player: {0} won the auction with the bid of {1}!" },
            { "AUCTION_STARTING", "Player {0} has started an auction! Type /previewauction to preview it. Bids starting at: {1}$" },
            { "BID", "Player: {0} has bid {1} on the current auction!" },
            { "LOST_AUCTION", "You lost the auction. Your {0}$ was returned." },
            { "AUCTION_NOT_RUNNING", "There's currently no auction running!" }
        };

        public static void Say(UnturnedPlayer player, string translation, Color color, params object[] args)
        {
            ChatManager.instance.channel.send("tellChat", player.CSteamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
            {
                CSteamID.Nil,
                Instance.Configuration.Instance.IconUrl,
                (byte)EChatMode.GLOBAL,
                color,
                false,
                Instance.Translate(translation, args)
            });
        }

        public static void Say(string translation, Color color, params object[] args)
        {
            ChatManager.instance.channel.send("tellChat", ESteamCall.OTHERS, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
            {
                CSteamID.Nil,
                Instance.Configuration.Instance.IconUrl,
                (byte)EChatMode.GLOBAL,
                color,
                false,
                Instance.Translate(translation, args)
            });
        }

        public static void SayText(string text, Color color)
        {
            ChatManager.instance.channel.send("tellChat", ESteamCall.OTHERS, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
            {
                CSteamID.Nil,
                Instance.Configuration.Instance.IconUrl,
                (byte)EChatMode.GLOBAL,
                color,
                false,
                text
            });
        }
    }
}
