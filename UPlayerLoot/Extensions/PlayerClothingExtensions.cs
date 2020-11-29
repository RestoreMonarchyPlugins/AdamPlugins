using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPlayerLoot.Serialization;

namespace UPlayerLoot.Extensions
{
    public static class PlayerClothingExtensions
    {
        public static IEnumerable<SerializableItem> GetClothing(this PlayerClothing source)
        {
            ICollection<SerializableItem> result = new List<SerializableItem>();
            if (source.backpack != default)
                result.Add(SerializableItem.CreateSerializableItem(new Item(source.backpack, 1, source.backpackQuality, source.backpackState)));
            if (source.backpack != default)

                result.Add(SerializableItem.CreateSerializableItem(new Item(source.glasses, 1, source.glassesQuality, source.glassesState)));
            if (source.hat != default)
                result.Add(SerializableItem.CreateSerializableItem(new Item(source.hat, 1, source.hatQuality, source.hatState)));
            if (source.mask != default)
                result.Add(SerializableItem.CreateSerializableItem(new Item(source.mask, 1, source.maskQuality, source.maskState)));
            if (source.pants != default)
                result.Add(SerializableItem.CreateSerializableItem(new Item(source.pants, 1, source.pantsQuality, source.pantsState)));
            if (source.shirt != default)
                result.Add(SerializableItem.CreateSerializableItem(new Item(source.shirt, 1, source.shirtQuality, source.shirtState)));
            if (source.vest != default)
                result.Add(SerializableItem.CreateSerializableItem(new Item(source.vest, 1, source.vestQuality, source.vestState)));

            return result;
        }
    }

}
