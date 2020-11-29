using Adam.InfoRestorer.Handlers;
using Adam.InfoRestorer.Players;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.InfoRestorer
{
    public class InfoRestorerPlugin : RocketPlugin<InfoRestorerConfiguration>
    {
        public const int ProductID = 138;
        public System.Version ProductVersion = new System.Version(1, 0, 0, 2); //Keep it the same when uploading to website!

        public static InfoRestorerPlugin Instance { get; private set; }
        protected override void Load()
        {
            Instance = this;
            base.Load();
            UnturnedPlayerEvents.OnPlayerDeath += OnDeath;
            U.Events.OnPlayerDisconnected += OnDisconnected;
            U.Events.OnPlayerConnected += OnPlayerConnected;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (!InfoHandler.Players.Exists(c => c.SteamID == player.CSteamID))
                InfoHandler.Players.Add(new PlayerSession(player));
        }

        private void OnDisconnected(UnturnedPlayer player)
        {
            if (Configuration.Instance.RemoveInfoOnLeave)
                InfoHandler.Players.RemoveAll(c => c.SteamID == player.CSteamID);
        }

        private void OnDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            var session = GetSession(player);
            session.SaveCurrent();
        }

        public PlayerSession GetSession(UnturnedPlayer player)
            => InfoHandler.Players.Find(c => c.SteamID == player.CSteamID);

        protected override void Unload()
        {
            base.Unload();
            UnturnedPlayerEvents.OnPlayerDeath -= OnDeath;
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {
            "invalid_syntax",
            "Invalid Syntax! Usage: /restore <player> <times ago>"
            },
            {
                "player_not_found",
                "Player not found!"
            },
            {
                "not_number",
                "{0} is not a number higer then 0!"
            },
            {
                "too_much",
                "{0} hasn't died that much!"
            },
            {
                "restored",
                "You've succesfully restored {0}'s inventory!"
            }
        };
    }
}
