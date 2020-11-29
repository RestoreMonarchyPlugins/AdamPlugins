using Adam.InfoRestorer.Serialization;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.InfoRestorer.Players
{
    public class PlayerSession
    {
        public CSteamID SteamID { get; }
        public PlayerSession(UnturnedPlayer player)
        {
            SteamID = player.CSteamID;
        }

        public UnturnedPlayer Player => UnturnedPlayer.FromCSteamID(SteamID);
        public List<InfoSlot> Slots = new List<InfoSlot>();

        public void SaveCurrent()
        {
            if (Slots.Count > InfoRestorerPlugin.Instance.Configuration.Instance.InfoStorageCapacity)
                Slots.Remove(Slots.First());
            Slots.Add(new InfoSlot(this));
        }
    }
}
