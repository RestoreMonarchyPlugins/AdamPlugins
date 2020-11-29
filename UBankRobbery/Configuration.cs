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
        public List<BankRobberyRegionConfiguration> Banks { get; set; }

        public void LoadDefaults()
        {
            Banks = new List<BankRobberyRegionConfiguration>()
            {
                new BankRobberyRegionConfiguration()
                {
                    MaximumReward = 5000,
                    MinimumReward = 2500,
                    RegionId = "region1",
                    RobbingDuration = 20,
                    RobbingInterval = 60
                }
            };
        }
    }
}
