using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPlayerLoot.Serialization;

namespace UPlayerLoot.Extensions
{
    public static class PlayerInventoryExtensions
    {
        public static IEnumerable<SerializableItem> GetItems(this PlayerInventory source)
        {
            List<SerializableItem> result = new List<SerializableItem>();
            for (byte page = 0; page < source.items.Length; page++)
            {
                if (page == PlayerInventory.AREA || page == PlayerInventory.STORAGE)
                    continue;
                var pageInstance = source.items[page];
                if (pageInstance == null)
                    continue;
                for (byte i = 0; i < pageInstance.getItemCount(); i++)
                {
                    var item = pageInstance.getItem(i);
                    if (item == null)
                        continue;

                    result.Add(SerializableItem.CreateSerializableItem(item.item));
                }
            }
            return result;
        }
    }
}
