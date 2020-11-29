using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UAuction.Serialization
{
    public class SerializableVehicle
    {
        public ushort VehicleId { get; set; }
        public ushort Health { get; set; }
        public ushort Fuel { get; set; }
        public ushort BatteryCharge { get; set; }
        public bool[] Tires { get; set; }
        public List<SerializableItem> TrunkSpace { get; set; } = new List<SerializableItem>();

        public List<SerializableBarricade> Barricades { get; set; } = new List<SerializableBarricade>();

        public static SerializableVehicle Create(InteractableVehicle vehicle)
        {
            var result = new SerializableVehicle();

            result.Health = vehicle.health;
            result.Fuel = vehicle.fuel;
            result.BatteryCharge = vehicle.batteryCharge;
            result.VehicleId = vehicle.id;
            result.TrunkSpace = vehicle.trunkItems?.items?.Select(c => SerializableItem.CreateSerializableItem(vehicle.trunkItems.page, c))?.ToList() ?? new List<SerializableItem>();

            result.Tires = vehicle.tires?.Select(c => c.isAlive)?.ToArray() ?? new bool[0];

            if (BarricadeManager.tryGetPlant(vehicle.transform, out byte x, out byte y, out ushort plant, out var region))
            {
                foreach (var data in region.barricades)
                {
                    if (data.barricade.isDead)
                        continue;

                    var drop = region.drops.FirstOrDefault(c => c.instanceID == data.instanceID);
                    var barricade = SerializableBarricade.Create(drop, data);
                    result.Barricades.Add(barricade);
                }
            }

            return result;
        }

        public InteractableVehicle SummonVehicle(Vector3 position, Quaternion rotation)
        {
            VehicleManager.spawnVehicleV2(VehicleId, position, rotation);
            var vehicle = VehicleManager.vehicles.Last();

            for (int i = 0; i < (Tires?.Length ?? 0); i++)
            {
                vehicle.tires[i].isAlive = Tires[i];
            }

            vehicle.sendTireAliveMaskUpdate();


            if (vehicle.trunkItems != null)
            {
                foreach (var item in TrunkSpace)
                {
                    var itemPos = item.Position.Value;

                    vehicle.trunkItems.addItem(itemPos.X, itemPos.Y, itemPos.Rot, item.ToItem());
                }
            }
            foreach(var barricade in Barricades)
            {
                barricade.SummonBarricade(vehicle.transform);
            }
            return vehicle;
        }
    }
}
