using Rocket.Core.Plugins;
using System;
using Rocket.API.Collections;
using SDG.Unturned;
using UnityEngine;
using System.Reflection;
using Rocket.Unturned.Player;
using Rocket.Unturned;
using System.Timers;
using HarmonyLib;
using Adam.PetsPlugin.Transelations;
using Adam.PetsPlugin.Handlers;
using UPets.Reflection;
using System.Collections;
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
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor)

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



            {  "no_lastpet", "Couden't manage to find your last used pet! Type (/pet help) for help! Color=red" },
            {  "cant_find_pet", "Coudn't manage to find a pet you own called {0} Color=red" },
            {  "max_time_reached", "Your pet has died respawn it again by /pet! Color=yellow" },
            {  "succesfully_spawned_pet", "You succesfully spawned your pet in! Color=yellow" },
            {  "all_pets", "All the available pets are: {0} Color=yellow" },
            {  "invalid_syntax", "/buy <pet> Color=red" },
            {  "cant_find_global_pet",  "Couden't manage to find a pet called {0} Color=red"},
            {  "cant_afford", "You can't afford to buy this pet for {0}. Color=red" },
            {  "succesfully_bought", "You have succefully bought a {0} for {1}! Color=yellow" },
            {  "already_own", "You already own this pet! COlor=red" },
            {  "succesfully_despawned", "You have succesfully despawned your pet! Color=yellow" },
            {  "using_no_pet", "You do not have a pet equipped at this moment! Color=yellow" },
            {  "succesfully_disabled_ui", "You have succesfully disabled the pets UI Information! Color=yellow" },
            {  "succesfully_enabled_ui", "You have succesfully enabled the pets UI Information! Color=yellow" },
            {  "help_line", "/pet buy <name> | /pet list | /pet toggleui | /pet despawn | /pet <name> | /pet (spawns the most recent pet) Color=yellow" }
        };
    }
}

