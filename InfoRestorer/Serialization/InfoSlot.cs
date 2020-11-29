using Adam.InfoRestorer.Players;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Adam.InfoRestorer.Serialization.ClothingUtil;

namespace Adam.InfoRestorer.Serialization
{
    public class InfoSlot
    {
        private PlayerSession session;
        public InfoSlot(PlayerSession session)
        {
            List<ItemWrapper> rs = new List<ItemWrapper>();
            this.session = session;
            Console.WriteLine(session.Player == null);
            rs.AddRange(session.Player.GetClothing());
            foreach (var items in session.Player.Inventory.items)
            {
                for (byte index = 0; index < items?.getItemCount(); index++)
                {

                    var item = items.getItem(index);
                    if (item?.item == null)
                        continue;
                    rs.Add(new ItemWrapper(item, items.page));
                }
            }
            items = rs;
        }

        public List<ItemWrapper> items = new List<ItemWrapper>();
        public void Load(bool clear)
        {
            var player = session.Player;
            if (clear)
                foreach (var items in player.Inventory.items)
                {
                    
                    if (items == null || items.page == PlayerInventory.AREA || items.page == PlayerInventory.STORAGE)
                        continue;
                    while (items?.getItemCount() > 0)
                        items?.removeItem(0);
                }

            foreach (var item in items)
            {
                item?.AddToInventory(player);
            }
        }
    }
}
