using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAuction.Serialization
{
    public struct SerializableItemPosition
    {
        public byte Page { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Rot { get; set; }
    }
}
