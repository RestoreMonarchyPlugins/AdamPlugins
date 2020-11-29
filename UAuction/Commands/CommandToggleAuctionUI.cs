using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UAuction.Commands
{
    public class CommandToggleAuctionUI : IRocketCommand
    {
        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Player;
        public string Name { get; } = "toggleauctionui";
        public string Help { get; } = "";
        public string Syntax { get; } = "";
        public List<string> Aliases { get; } = new List<string>()
        {
            "toggleaucui"
        };
        public List<string> Permissions { get; } = new List<string>()
        {
            "toggleauctionui"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            var session = Plugin.Instance.SessionManager.Sessions.FirstOrDefault(c => c.Player.channel.owner.playerID.steamID == player.CSteamID);
            session.IsUiEnabled = !session.IsUiEnabled;
            Plugin.Say(player, "UI_TOGGLED", Color.green, session.IsUiEnabled);
            if (!session.IsUiEnabled)
            {
                EffectManager.instance.channel.send("tellEffectClearByID", player.CSteamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, Plugin.Instance.Configuration.Instance.UiEffect);
            }
            else
            {
                Plugin.Instance.UIManager.SpawnUi(player.CSteamID);
            }
        }
    }
}
