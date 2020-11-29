using HarmonyLib;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UPlayerLoot.Patches
{
    [HarmonyPatch(typeof(PlayerInventory), "askInventory")]
    public static class AskInventoryPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerInventory __instance, CSteamID steamID)
        {

            if (!__instance.channel.checkOwner(steamID))
                return false;

            __instance.channel.openWrite();

            var found = Plugin.Instance.InfoSerializer.Players.FirstOrDefault(c => c.Id == __instance.channel.owner.playerID.steamID.m_SteamID);
            var character = found?.Characters?.FirstOrDefault(c => c.CharacterId == __instance.channel.owner.playerID.characterID);


            if (character == null)
                return true; //No previous login detected so just let it behave as normal.
            bool result = true;

            found.Characters.Remove(character);

            if (!character.Destroy()) //IF it is destroyed then the player should have no items.
            {

                for (byte page = 0; (int)page < (int)PlayerInventory.PAGES - 2; ++page)
                {
                    var pageInstance = __instance.items[page];
                    pageInstance.items.Clear();

                    __instance.channel.write((object)__instance.items[(int)page].width, (object)__instance.items[(int)page].height, (object)__instance.items[(int)page].getItemCount());
                }
                __instance.channel.closeWrite("tellInventory", steamID, ESteamPacket.UPDATE_RELIABLE_CHUNK_BUFFER);

                __instance.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    (byte)0, (byte)0, new byte[0]);
                __instance.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    (byte)1, (byte)0, new byte[0]);
                __instance.items[2].resize((byte)5, (byte)3);
                if(Plugin.Instance.Configuration.Instance.ClearClothing)
                    __instance.player.clothing.channel.send("tellClothing", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, 0,0, new byte[0], 0, 0, new byte[0], 0, 0, new byte[0], 0, 0, new byte[0], 0, 0, new byte[0], 0, 0, new byte[0], 0, 0, new byte[0], __instance.player.clothing.isVisual, __instance.player.clothing.isSkinned, __instance.player.clothing.isMythic);

                UnturnedChat.Say(UnturnedPlayer.FromCSteamID(steamID), Plugin.Instance.Translate("DESTROYED"));
                result = false;

            }


            Plugin.Instance.InfoSerializer.Save();

            //Don't remove it yet, remove it in connect so we can message the player.
            return result;
        }

    }
}
