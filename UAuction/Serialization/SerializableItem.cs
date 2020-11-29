using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAuction.Serialization
{
    public class SerializableItem
    {
        public ushort ItemId { get; set; }
        public byte Amount { get; set; }
        public byte Quality { get; set; }
        public byte[] State { get; set; }

        public SerializableItemPosition? Position { get; set; }

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

        public static SerializableItem CreateSerializableItem(byte page, ItemJar item)
        {
            var result = CreateSerializableItem(item.item);
            result.Position = new SerializableItemPosition()
            {
                Page = page,
                Rot = item.rot,
                X = item.x,
                Y = item.y
            };
            return result;
        }

        public Item ToItem()
            => new Item(ItemId, Amount, Quality, State);
    }

}