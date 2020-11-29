using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAuction.Patches
{
    [HarmonyPatch(typeof(PlayerAnimator), "askGesture")]
    public static class AskGesturePatch
    {
        public static bool Prefix(PlayerAnimator __instance, CSteamID steamID, byte id)
        {
            var session = Plugin.Instance.SessionManager.Sessions.FirstOrDefault(c => c.Player.channel.owner.playerID.steamID == __instance.player.channel.owner.playerID.steamID);
            session?.OnGesture((EPlayerGesture)id);
            return true;
        }
    }
}
