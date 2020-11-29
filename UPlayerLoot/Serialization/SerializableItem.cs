using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPlayerLoot.Serialization
{
    public class SerializableItem
    {
        public ushort ItemId { get; set; }
        public byte Amount { get; set; }
        public byte Quality { get; set; }
        public byte[] State { get; set; }

        public static SerializableItem CreateSerializableItem(Item item)
        {
            return new SerializableItem()
            {
                ItemId = item.id,
                Amount = item.amount,
                Quality = item.quality,
                State = item.state,
            };
        }

    }
}
