using Rocket.API;

namespace UPlayerLoot
{
    public class Configuration : IRocketPluginConfiguration
    {
        public ushort MannequinId { get; set; }
        public bool ClearClothing { get; set; }
        public void LoadDefaults()
        {
            MannequinId = 1409;
            ClearClothing = false;
        }
    }
}