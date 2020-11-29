using HarmonyLib;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentryCommander
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public const string HarmonyInstanceId = "de.sentrycommander";
        private Harmony HarmonyInstance { get; set; }

        protected override void Load()
        {
            HarmonyInstance = new Harmony(HarmonyInstanceId);
            HarmonyInstance.PatchAll(Assembly);
        }

        protected override void Unload()
        {
            HarmonyInstance.UnpatchAll(HarmonyInstanceId);
        }
    }
}
