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
        private const string PathPrefab = "Plugins/UPlayerLoot/Storage/{0}-info.json";
        public PlayerInfoSerializer()
        {

        }
        public List<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();

        public void Save()
        {
            string path = string.Format(PathPrefab, Level.info.name);
            string dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using(StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                writer.Write(JsonConvert.SerializeObject(new ListWrapper()
                {
                    Players = Players
                }));
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

    public class ListWrapper //Dont bother making anything, im just lazy
    {
        public List<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();
    }
}
