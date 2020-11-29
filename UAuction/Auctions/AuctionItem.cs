using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAuction.Serialization;

namespace UAuction.Auctions
{
    public class AuctionItem
    {
        public List<SerializableItem> Items { get; set; } = new List<SerializableItem>();
        public SerializableVehicle Vehicle { get; set; }
    }
}
