using Adam.PetsPlugin.Handlers;
using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.PetsPlugin.Patches
{
    [HarmonyPatch(typeof(AnimalManager))]
    class AnimalManagerPatches
    {
        [HarmonyPatch("askAnimalAttack")]
        [HarmonyPrefix]
        public static bool Prefix(CSteamID steamID, ushort index)
        {
            var item = PetHandler.PlayerPets.Find(c => (AnimalManager.animals.IndexOf(c.Animal) == index));
            if (item == null)
                return true;
            return false;
        }
    }
}
