using Rocket.API.Collections;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBankRobbery.Functionality;
using UBankRobbery.Regions;

namespace UBankRobbery
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance { get; private set; }
        public UBankRobbery.Regions.IRegionManager RegionManager { get; private set; }
        public RobManager RobManager { get; private set; }
        protected override void Load()
        {
            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
            Instance = this;


            RobManager = new RobManager();
            if (IsDependencyLoaded("AdvancedRegions"))
            {
                Rocket.Core.Logging.Logger.Log("Advanced regions found!");
                //RegionManager = new UBankRobbery.Regions.AdvancedRegions.AdvancedRegionsManager();
            }
            else if (IsDependencyLoaded("RocketRegions"))
            {
                Rocket.Core.Logging.Logger.Log("Rocket regions found!");
                RegionManager = new UBankRobbery.Regions.RocketRegions.RocketRegionsManager();
            }
            else
            {
                Rocket.Core.Logging.Logger.LogError("No regions plugin was found!");
                UnloadPlugin();
            }
        }
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {
                "succesfully_started",
                "{0} is getting robbed by {1}!"
            },
            {
                "ended",
                "{0} robbery on {1} has failed!"
            },
            {
                "finished",
                "{0} robbery succeded and he got away!"
            },
            {
                "no_region_found",
                "There is no bank here! color=red"
            },
            {
                "on_cooldown",
                "You're not allowed to rob this bank for another {0} seconds!"
            },
            {
                "already_robbing",
                "{0} is already getting robbed!"
            },
            {
                "robbing",
                "{0} is robbing bank {1}!"
            }
        };

        private void FixedUpdate()
        {
            this.RobManager.FixedUpdate();
        }
    }
}
