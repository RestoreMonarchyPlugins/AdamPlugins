using Adam.PetsPlugin.StorageItems;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.PetsPlugin.Handlers
{
    public static class PetHandler
    {
        public static Adam.PetsPlugin.Plugin PetsPlugin = Plugin.Instance;


        public static List<PlayerItem> PlayerPets { get; } = new List<PlayerItem>();

        public static Animal spawnPet(UnturnedPlayer player, PetAsset asset)
        {
            return spawnPet(player, asset, out var item);
        }

        public static bool despawnpet(UnturnedPlayer player)
        {
            if (player == null) return false;
            var pet = PlayerPets.Find(c => (c.Player == player.Player));
            if (pet == null) return false;
            pet.DespawnAnimal();
            return true;
        }

        public static Animal spawnPet(UnturnedPlayer player, PetAsset asset, out PlayerItem pitem)
        {
            pitem = null;
            if (player == null || asset == null) return null;
            pitem = PlayerPets.Find(c => (c.Player == player.Player));
            if (pitem == null)
            {
                PlayerPets.Add(new PlayerItem(player));
                pitem = PlayerPets[PlayerPets.Count - 1];
            }
            pitem.SpawnAnimal(asset);
            return pitem.Animal;
        }

        public static Player GetTarget(Animal animal)
        {
            var item =  PlayerPets.Find(c => (c.Animal == animal));
            if (item != null)
            {
                return item.Player;
            }
            return null;
        }

    }
}
