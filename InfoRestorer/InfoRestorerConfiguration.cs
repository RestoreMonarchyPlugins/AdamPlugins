using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.InfoRestorer
{
    public class InfoRestorerConfiguration : IRocketPluginConfiguration
    {
        public string LicenseKey;
        public int InfoStorageCapacity;
        public bool RemoveInfoOnLeave;
        public bool ShouldClearInventory;

        public void LoadDefaults()
        {
            LicenseKey = Guid.Empty.ToString();
            ShouldClearInventory = true;
            InfoStorageCapacity = 30;
            RemoveInfoOnLeave = true;
        }
    }
}
