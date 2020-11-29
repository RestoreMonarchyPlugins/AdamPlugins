using fr34kyn01535.Uconomy;
using Newtonsoft.Json;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAuction.Auctions;
using UAuction.Serialization;
using UnityEngine;

namespace UAuction.Awards
{
    public class AwardManager
    {
        private const string BasePath = "Plugins/UAuction/{0}-Tasks.json";
        public AwardManager()
        {
            U.Events.OnPlayerConnected += Connected;
        }

        public List<PlayerJoinedTask> Tasks { get; set; } = new List<PlayerJoinedTask>();

        public void Load()
        {
            var path = string.Format(BasePath, Level.info.name);
            if (File.Exists(path))
                Tasks = JsonConvert.DeserializeObject<List<PlayerJoinedTask>>(File.ReadAllText(path));
        }

        public void Save()
        {
            var path = string.Format(BasePath, Level.info.name);
            using(StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                writer.Write(JsonConvert.SerializeObject(Tasks));
            }
        }

        public void RewardPlayer(ulong id, AuctionItem items)
        {
            var player = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(id));
            if(player?.Player == null)
            {
                Tasks.Add(new PlayerJoinedTask()
                {
                    Id = id,
                    Items = items
                });
                Save();
            }
            else
            {
             
                foreach(var item in items.Items)
                {
                    var bakedItem = item.ToItem();
                    if (!player.Inventory.tryAddItem(bakedItem, true, true))
                        ItemManager.dropItem(bakedItem, player.Position, true, true, true);
                }
                Vector3 point = player.Player.transform.position + player.Player.transform.forward * 6f;
                point += Vector3.up * 16f;
                if (items.Vehicle != null)
                    items.Vehicle.SummonVehicle(point, player.Player.transform.rotation);
            }
        }

        public void RewardPlayer(ulong id, decimal amount)
        {
            Uconomy.Instance.Database.IncreaseBalance(id.ToString(), amount);
        }

        private void Connected(UnturnedPlayer player)
        {
            var tasks = Tasks.Where(c => c.Id == player.CSteamID.m_SteamID);
            foreach(var task in tasks)
            {
                foreach(var item in task.Items.Items)
                {
                    if (!player.Inventory.tryAddItem(item.ToItem(), true))
                        ItemManager.dropItem(item.ToItem(), player.Position, true, true, true);
                }

                Vector3 point = player.Player.transform.position + player.Player.transform.forward * 6f;
                point += Vector3.up * 16f;
                if (task.Items.Vehicle != null)
                    task.Items.Vehicle.SummonVehicle(point, player.Player.transform.rotation);

                Tasks.Remove(task);
            }
            
        }
    }

    public class PlayerJoinedTask
    {
        public ulong Id { get; set; }
        public AuctionItem Items { get; set; }
    }
}
