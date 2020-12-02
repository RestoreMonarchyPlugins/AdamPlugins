using Rocket.Core.Plugins;
using Rocket.API.Collections;
using SDG.Unturned;
using System.Reflection;
using HarmonyLib;
using Adam.PetsPlugin.Providers;
using Adam.PetsPlugin.Services;
using Rocket.Unturned.Chat;

namespace Adam.PetsPlugin
{
    public class PetsPlugin : RocketPlugin<PetsConfiguration>
    {
        public static PetsPlugin Instance { get; private set; }
        public UnityEngine.Color MessageColor { get; private set; }

        public const string HarmonyInstanceId = "de.petsplugin";
        private Harmony HarmonyInstance { get; set; }

        public IPetsDatabaseProvider Database { get; private set; }

        public PetsService PetsService { get; private set; }
        public PetsMovementService PetsMovementService { get; private set; }

        private FieldInfo animalManagerField;
        public AnimalManager AnimalManager => animalManagerField.GetValue(null) as AnimalManager;

        protected override void Load()
        {
            Instance = this;
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, UnityEngine.Color.green);

            animalManagerField = typeof(AnimalManager).GetField("manager", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static);

            HarmonyInstance = new Harmony(HarmonyInstanceId);
            HarmonyInstance.PatchAll(Assembly);

            Database = new JsonPetsDatabaseProvider();
            Database.Reload();

            PetsService = gameObject.AddComponent<PetsService>();
            PetsMovementService = gameObject.AddComponent<PetsMovementService>();
        }

        protected override void Unload()
        {
            Instance = null;

            HarmonyInstance.UnpatchAll(HarmonyInstanceId);

            Destroy(PetsService);
            Destroy(PetsMovementService);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "PetHelpLine1", "/pet list - Displays a list of your pets" },
            { "PetHelpLine2", "/pet buy <name> - Buys a pet with specified name" },
            { "PetHelpLine3", "/pet shop - Displays a list of available pets in the shop" },
            { "PetHelpLine4", "/pet <name> - Spawns/Despawns a specified pet" },            
            { "PetShopAvailable", "Available pets:" },
            { "PetShopNoPets", "There isn't any pet available in the shop" },
            { "PetList", "Your Pets: {0}" },
            { "PetListNone", "You don't have any pets" },
            { "PetNameRequired", "You have to specify pet name" },
            { "PetNotFound", "Failed to find any pet called {0}" },
            { "PetSpawnSuccess", "Successfully spawned {0}!" },
            { "PetSpawnFail", "You don't have {0}" },
            { "PetDespawnSuccess", "Successfully despawned your {0}!" },
            { "PetCantAfford", "You can't afford to buy {0} for ${1}" },
            { "PetBuySuccess", "You successfully bought {0} for ${1}!" },
            { "PetBuyAlreadyHave", "You already have {0}!" }
        };
    }
}

