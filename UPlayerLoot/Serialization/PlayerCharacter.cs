using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UPlayerLoot.Serialization
{
    public class PlayerCharacter
    {

        public byte CharacterId { get; set; }
        public List<SerializableItem> Items { get; set; }
        public Vector3Wrapper BarricadeLocation { get; set; }
        public uint InstanceId { get; set; }

        public bool Destroy()
        {
            var vector = BarricadeLocation.ToVector3();
            if (!Regions.tryGetCoordinate(vector, out byte x, out byte y))
                return true;
            var region = BarricadeManager.regions[x, y];
            var drop = region.drops.FirstOrDefault(c => c.instanceID == InstanceId);
            var barr = region.barricades.FirstOrDefault(c => c.instanceID == InstanceId);

            if (drop == null || barr.barricade.isDead) //Not found
                return false;

            BarricadeManager.destroyBarricade(region, x, y, ushort.MaxValue, (ushort)region.drops.IndexOf(drop));

            return true;
        }

        public bool IsBarricadeDestroyed()
        {
            var vector = BarricadeLocation.ToVector3();
            if (!Regions.tryGetCoordinate(vector, out byte x, out byte y))
                return true;

            var region = BarricadeManager.regions[x, y];
            var data = region.barricades.FirstOrDefault(c => c.instanceID == InstanceId);
            return data?.barricade?.isDead ?? true;
        }

        public void SummonContainer(PlayerClothing clothing, ushort id)
        {

            var transform = BarricadeManager.dropBarricade(new Barricade(id), null, BarricadeLocation.ToVector3(), 0, 0, 0, 0, 0)
                .GetComponent<InteractableMannequin>();
            if (!Plugin.Instance.Configuration.Instance.ClearClothing)
            {
                transform.clothes.visualShirt = clothing.shirt;
                transform.clothes.visualPants = clothing.pants;
                transform.clothes.visualHat = clothing.hat;
                transform.clothes.visualBackpack = clothing.backpack;
                transform.clothes.visualVest = clothing.vest;
                transform.clothes.visualMask = clothing.mask;
                transform.clothes.visualGlasses = clothing.glasses;
            }

            transform.rebuildState();
            BarricadeManager.tryGetInfo(transform.transform, out byte x, out byte y, out ushort plant, out ushort index, out var region, out var drop);
            InstanceId = drop.instanceID;
        }

    }
}