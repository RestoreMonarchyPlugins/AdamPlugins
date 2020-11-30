using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.PetsPlugin.Models
{
    public class PlayerPet
    {
        public int Id { get; set; }
        public string PlayerId { get; set; }
        public ushort PetId { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}