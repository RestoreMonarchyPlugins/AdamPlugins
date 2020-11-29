﻿using Adam.PetsPlugin.Handlers;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using UPets.Reflection;

namespace Adam.PetsPlugin.StorageItems
{
    public class PlayerItem : IDisposable
    {

        public PlayerItem(UnturnedPlayer player)
        {
            if (player == null) return;
            this.Player = player.Player;
        }

        public Player Player { get; }
        public Animal Animal { get; private set; }
        public DateTime SummonedAt { get; private set; }
        public PetAsset Asset { get; private set;}
        public void SpawnAnimal(PetAsset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));
            if (Animal != null)
                DespawnAnimal();

            Animal = spawnAnimal(asset.id, Player.transform.position, Player.transform.rotation);
            Asset = asset;
            SummonedAt = DateTime.UtcNow;
        }

        public bool DespawnAnimal()
        {
            if (Animal == null)
                return false;
            Plugin.Instance.sendAnimalRemove(Animal);
            Animal = null;
            return true;
        }

        private Animal addAnimal(ushort id, Vector3 point, float angle)
        {
            return (Animal)typeof(AnimalManager).GetMethod("addAnimal", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Plugin.Instance.AInstance, new object[]
            {
                id,
                point,
                angle,
                false
            });
        }

        private Animal spawnAnimal(ushort id, Vector3 point, Quaternion angle)
        {
            Animal animal1 = addAnimal(id, point, angle.eulerAngles.y);

            PackInfo packInfo = new PackInfo();
            animal1.pack = packInfo;
            packInfo.animals.Add(animal1);
            Plugin.Instance.AInstance.channel.openWrite();
            Plugin.Instance.AInstance.sendAnimal(animal1);
            Plugin.Instance.AInstance.channel.closeWrite("tellAnimal", ESteamCall.OTHERS, ESteamPacket.UPDATE_RELIABLE_CHUNK_BUFFER);
            AnimalManager.sendAnimalAlive(animal1, point, (byte)angle.y);
            return animal1;
        }



        public void Dispose()
        {
            DespawnAnimal();
        }
    }

}