using Adam.PetsPlugin.Helpers;
using Adam.PetsPlugin.Models;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Core.Extensions;
using UnityEngine;

namespace Adam.PetsPlugin.Services
{
    public class PetsService : MonoBehaviour
    {
        private PetsPlugin pluginInstance => PetsPlugin.Instance;
        
        public event PetSpawned OnPetSpawned;
        public event PetDespawned OnPetDespawned;
        
        public List<PlayerPet> ActivePets { get; private set; }

        void Awake()
        {
            ActivePets = new List<PlayerPet>();
        }

        void Start()
        {
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        void OnDestroy()
        {
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            
            //we want to also kill all pets on shutdown
            foreach (var pet in ActivePets)
            {
                KillPet(pet);
            }
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            foreach (var pet in GetPlayerActivePets(player.Id).ToArray())
            {
                KillPet(pet);
            }
        }
        
        public void SpawnPet(UnturnedPlayer player, PlayerPet pet)
        {
            foreach (var activePet in GetPlayerActivePets(player.Id).ToArray())
            {
                KillPet(activePet);
            }

            pet.Animal = AnimalsHelper.SpawnAnimal(pet.AnimalId, player.Position, (byte)player.Rotation);
            pet.Player = player.Player;
            ActivePets.Add(pet);
            OnPetSpawned.TryInvoke(pet);
        }

        public void KillPet(PlayerPet pet)
        {
            AnimalsHelper.KillAnimal(pet.Animal);
            ActivePets.Remove(pet);
            OnPetDespawned.TryInvoke(pet);
        }

        public bool IsPet(Animal animal) => ActivePets.Exists(x => x.Animal == animal);
        public PlayerPet GetPet(Animal animal) => ActivePets.FirstOrDefault(x => x.Animal == animal);
        public IEnumerable<PlayerPet> GetPlayerActivePets(string playerId) => ActivePets.Where(x => x.PlayerId == playerId);
    }

    public delegate void PetSpawned(PlayerPet pet);
    public delegate void PetDespawned(PlayerPet pet);
}
