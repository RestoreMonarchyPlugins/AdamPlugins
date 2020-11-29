using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.QueueBypasser;
using Adam.QueueBypasser.Patches;
using HarmonyLib;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Adam.QueueBypasser
{
    public class QueueBypasserPlugin : RocketPlugin<QueueBypasserConfiguration>
    {
        public const int ProductID = 151;
        public Version ProductVersion = new Version(1, 0, 0, 4); //Keep it the same when uploading to website!


        public Harmony Harmony { get; private set; }
        public static QueueBypasserPlugin Instance { get; private set; }


        protected override void Load()
        {
            base.Load();
            Instance = this;

            Harmony = new Harmony("de.adam.queuebypasser");

            var originalmethod = typeof(Provider).GetMethod("receiveServer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var prefix = typeof(ReceiveServerPatch).GetMethod("Prefix");
            Harmony.Patch(originalmethod, new HarmonyMethod(prefix), null, null);

        }

        protected override void Unload()
        {
            base.Unload();
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {
                "none",
                "none"
            }
        };

    }
}
