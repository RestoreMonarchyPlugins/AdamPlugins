using Newtonsoft.Json;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPlayerLoot.Serialization
{
    public class PlayerInfoSerializer
    {
        public string PathPrefab { get; private set; }

        public PlayerInfoSerializer(string directory, string fileName)
        {
            PathPrefab = Path.Combine(directory, fileName);
        }
        public List<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();

        public void Save()
        {
            string path = string.Format(PathPrefab, Level.info.name);
            
            using(StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                writer.Write(JsonConvert.SerializeObject(new ListWrapper()
                {
                    Players = Players
                }, Formatting.Indented));
            }
        }

        public void Load()
        {
            string path = string.Format(PathPrefab, Level.info.name);

            if (!File.Exists(path))
                return;
            Players = JsonConvert.DeserializeObject<ListWrapper>(File.ReadAllText(path)).Players;
        }
    }

    public class ListWrapper
    {
        public List<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();
    }
}
