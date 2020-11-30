using Adam.PetsPlugin.Models;
using Adam.PetsPlugin.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.PetsPlugin.Providers
{
    public class JsonPetsDatabaseProvider : IPetsDatabaseProvider
    {
        private PetsPlugin pluginInstance => PetsPlugin.Instance;

        private DataStorage<List<PlayerPet>> DataStorage { get; set; }

        public JsonPetsDatabaseProvider()
        {
            DataStorage = new DataStorage<List<PlayerPet>>(pluginInstance.Directory, "PlayersPets.json");
        }

        private List<PlayerPet> playersPets;
        private int GetIDForPet() => playersPets.OrderBy(x => x.Id).LastOrDefault()?.Id + 1 ?? 1;
        
        public void Reload()
        {
            playersPets = DataStorage.Read();
            if (playersPets == null)
                playersPets = new List<PlayerPet>();
        }

        public void AddPlayerPet(PlayerPet playerPet)
        {
            playerPet.Id = GetIDForPet();
            playersPets.Add(playerPet);
        }

        public void DeletePlayerPet(int id)
        {
            if (playersPets.RemoveAll(x => x.Id == id) > 0)
                DataStorage.Save(playersPets);
        }

        public IEnumerable<PlayerPet> GetPlayerPets(string playerId)
        {
            return playersPets.Where(x => x.PlayerId == playerId); 
        }
    }
}
