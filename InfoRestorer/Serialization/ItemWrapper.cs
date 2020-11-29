using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Adam.InfoRestorer.Serialization
{
    public static class ClothingUtil
    {
        public static List<ItemWrapper> GetClothing(this UnturnedPlayer player)
        {
            var list = new List<ItemWrapper>();
            PlayerClothing clothing = player.Player.clothing;

            if (clothing.backpack != 0)
            {
                ItemWrapper item = new ItemWrapper(new Item(clothing.backpack, 1, clothing.backpackQuality, clothing.backpackState));
                list.Add(item);
            }

            if (clothing.glasses != 0)
            {
                ItemWrapper item = new ItemWrapper(new Item(clothing.glasses, 1, clothing.glassesQuality, clothing.glassesState));
                list.Add(item);
            }


            if (clothing.hat != 0)
            {
                ItemWrapper item = new ItemWrapper(new Item(clothing.hat, 1, clothing.hatQuality, clothing.hatState));
                list.Add(item);
            }

            if (clothing.mask != 0)
            {
                ItemWrapper item = new ItemWrapper(new Item(clothing.mask, 1, clothing.maskQuality, clothing.maskState));
                list.Add(item);
            }

            if (clothing.shirt != 0)
            {
                ItemWrapper item = new ItemWrapper(new Item(clothing.shirt, 1, clothing.shirtQuality, clothing.shirtState));
                list.Add(item);
            }

            if (clothing.vest != 0)
            {
                ItemWrapper item = new ItemWrapper(new Item(clothing.vest, 1, clothing.vestQuality, clothing.vestState));
                list.Add(item);
            }

            if(clothing.pants != 0)
            {
                ItemWrapper item = new ItemWrapper(new Item(clothing.pants, 1, clothing.pantsQuality, clothing.pantsState));
                list.Add(item);
            }
            return list;
        }
        public struct Vector2Byte
        {
            public Vector2Byte(byte x, byte y)
            {
                this.x = x;
                this.y = y;
            }
            public byte x;
            public byte y;
        }
        public class ItemWrapper
        {
            public byte[] State { get; set; }
            public ushort ID { get; set; }
            public byte Amount { get; set; }
            public Vector2Byte Position { get; set; }
            public byte? Page { get; set; }
            public byte Quality { get; set; }
            public byte Rot { get; set; }
            public bool IsClothing => Page == null;

            public ItemWrapper(ItemJar item, byte? page = null) : this(item.item, page, new Vector2Byte(item.x, item.y))
            {

            }
            public ItemWrapper(Item item, byte? page = null, Vector2Byte? position = null)
            {
                Page = page;
                State = item.metadata;
                ID = item.id;
                Amount = item.amount;
                Quality = item.quality;
            }

            public void AddToInventory(UnturnedPlayer player)
            {
                if (IsClothing)
                {
                    player.Player.inventory.forceAddItem(new Item(ID, Amount, Quality, State), true);
                    return;
                }
                if (!player.Inventory.tryAddItem(new Item(ID, Amount, Quality, State), Position.x, Position.y, Page.Value, Rot))
                {
                    player.Inventory.forceAddItem(new Item(ID, Amount, Quality, State), true);
                }
            }
        }
    }
}
