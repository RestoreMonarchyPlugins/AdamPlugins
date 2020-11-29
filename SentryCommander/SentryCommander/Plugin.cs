
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
        public Harmony HarmonyInstance { get; } = new Harmony("de.sentrycommander");

        protected override void Load()
        {
            base.Load();
            HarmonyInstance.PatchAll(Assembly);
        }

        protected override void Unload()
        {
            base.Unload();
        }
    }
}
