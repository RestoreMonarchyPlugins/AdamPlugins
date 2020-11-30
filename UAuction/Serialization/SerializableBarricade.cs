using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UAuction.Serialization
{
    public class SerializableBarricade
    {

        public Vector3Wrapper Position { get; set; }
        public Vector3Wrapper Rotation { get; set; }
        public byte[] State { get; set; }

        public ulong Owner { get; set; }
        public ulong Group { get; set; }
        public uint ActiveDate { get; set; }

        public ushort Health { get; set; }
        public ushort Id { get; set; }

        public static SerializableBarricade Create(BarricadeDrop drop, BarricadeData data)
        {
            SerializableBarricade result = new SerializableBarricade();
            
            result.Position = new Vector3Wrapper(data.point);
            result.Rotation = new Vector3Wrapper(drop.model.transform.localEulerAngles);

            result.Owner = data.owner;
            result.Group = data.group;
            result.State = data.barricade.state;
            result.ActiveDate = data.objActiveDate;
            result.Health = data.barricade.health;
            result.Id = data.barricade.id;

            return result;
        }

        public Transform SummonBarricade(Transform hit)
        {
            var barricade = new Barricade(Id, Health, State, (ItemBarricadeAsset)Assets.find(EAssetType.ITEM, Id));
            if (hit != null)
            {
                return BarricadeManager.dropPlantedBarricade(hit, barricade, Position.ToVector3(), Quaternion.Euler(Rotation.x, Rotation.y, Rotation.z), Owner, Group);
            }
            else
            {
                return BarricadeManager.dropNonPlantedBarricade(barricade, Position.ToVector3(), Quaternion.Euler(Rotation.x, Rotation.y, Rotation.z), Owner, Group);
            }
        }
    }
}
