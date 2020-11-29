using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathfinding.Util;
using Rocket.API;

namespace Adam.QueueBypasser
{
    public class QueueBypasserConfiguration : IRocketPluginConfiguration
    {
        public string LicenseKey;

        public void LoadDefaults()
        {
            LicenseKey = System.Guid.Empty.ToString();
        }
    }
}
