using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAuction.Utilities;

namespace UAuction.Auctions
{
    public class AuctionBid
    {
        public CachedPlayer Player { get; set; }
        public decimal Amount { get; set; }
    }
}
