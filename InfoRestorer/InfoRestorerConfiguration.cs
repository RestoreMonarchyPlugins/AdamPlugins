using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.InfoRestorer
{
    public class InfoRestorerConfiguration : IRocketPluginConfiguration
    {
        public int InfoStorageCapacity { get; set; }
        public bool RemoveInfoOnLeave { get; set; }
        public bool ShouldClearInventory { get; set; }

        public void LoadDefaults()
        {
            ShouldClearInventory = true;
            InfoStorageCapacity = 30;
            RemoveInfoOnLeave = true;
        }
    }
}
