using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBankRobbery.Regions;

namespace UBankRobbery
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool UseUconomy { get; set; }
        public List<BankRobberyRegionConfiguration> Banks { get; set; }

        public void LoadDefaults()
        {
            UseUconomy = false;
            Banks = new List<BankRobberyRegionConfiguration>()
            {
                new BankRobberyRegionConfiguration()
                {
                    MaximumReward = 5000,
                    MinimumReward = 2500,
                    RegionId = "bank1",
                    RobbingDuration = 20,
                    RobbingInterval = 60
                }
            };
        }
    }
}
