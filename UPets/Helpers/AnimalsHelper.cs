using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Adam.PetsPlugin.Helpers
{
    public class AnimalsHelper
    {
        private static PetsPlugin pluginInstance => PetsPlugin.Instance;

        public static Animal SpawnAnimal(ushort animalId, Vector3 point, byte angle)
        {
            Animal animal = AddAnimal(animalId, point, angle);
            AnimalSpawnpoint item = new AnimalSpawnpoint(0, point);
            var packInfo = new PackInfo();
            animal.pack = packInfo;
            packInfo.animals.Add(animal);
            packInfo.spawns.Add(item);
            //AnimalManager.packs.Add(packInfo);
            pluginInstance.AnimalManager.channel.openWrite();
            pluginInstance.AnimalManager.sendAnimal(animal);
            pluginInstance.AnimalManager.channel.closeWrite("tellAnimal", ESteamCall.OTHERS, ESteamPacket.UPDATE_RELIABLE_CHUNK_BUFFER);
            AnimalManager.sendAnimalAlive(animal, point, angle);
            
            return animal;
        }

        public static void KillAnimal(Animal animal)
        {
            AnimalManager.sendAnimalDead(animal, Vector3.zero);
        }

        private static Animal AddAnimal(ushort animalId, Vector3 point, float angle)
        {
            return (Animal)typeof(AnimalManager).GetMethod("addAnimal", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(pluginInstance.AnimalManager, new object[]
            {
                animalId,
                point,
                angle,
                false
            });
        }
    }
}
