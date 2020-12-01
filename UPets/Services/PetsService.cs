using Adam.PetsPlugin.Helpers;
using Adam.PetsPlugin.Models;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Adam.PetsPlugin.Services
{
    public class PetsService : MonoBehaviour
    {
        private PetsPlugin pluginInstance => PetsPlugin.Instance;

        public List<PlayerPet> ActivePets { get; private set; }

        void Awake()
        {
            ActivePets = new List<PlayerPet>();
        }

        void Start()
        {

        }

        void OnDestroy()
        {

        }

        public bool TrySpawnPet(UnturnedPlayer player, ushort petId)
        {
            var ownedPets = pluginInstance.Database.GetPlayerPets(player.Id);

            var pet = ownedPets.FirstOrDefault(x => x.AnimalId == petId);
            if (pet == null)
            {
                return false;
            } else
            {
                SpawnPet(player, pet);
                return true;
            }
        }
        
        private void SpawnPet(UnturnedPlayer player, PlayerPet pet)
        {
            pet.Animal = AnimalsHelper.SpawnAnimal(pet.AnimalId, player.Position, (byte)player.Rotation);
            pet.Player = player.Player;
            ActivePets.Add(pet);
        }

        private void KillPet(PlayerPet pet)
        {
            AnimalsHelper.KillAnimal(pet.Animal);
            ActivePets.Remove(pet);
        }

        public bool IsPet(Animal animal) => ActivePets.Exists(x => x.Animal == animal);
        public PlayerPet GetPet(Animal animal) => ActivePets.FirstOrDefault(x => x.Animal = animal);
        public IEnumerable<PlayerPet> GetPlayerActivePets(string playerId) => ActivePets.Where(x => x.PlayerId == playerId);
    }
}
