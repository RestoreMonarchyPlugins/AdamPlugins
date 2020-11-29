using Rocket.Core.Plugins;
using System;
using Rocket.API.Collections;
using SDG.Unturned;
using UnityEngine;
using System.Reflection;
using Steamworks;
using Rocket.Unturned.Player;
using Rocket.Unturned;
using System.Timers;
using HarmonyLib;
using Adam.PetsPlugin.Transelations;
using Adam.PetsPlugin.Handlers;
using UPets.Reflection;
using System.Collections;

namespace Adam.PetsPlugin
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance;
        private System.Timers.Timer timer;
        public Harmony Harmony { get; } = new Harmony("de.petsplugin");
        private DateTime _check = DateTime.Now;


        public const int ProductID = 123;
        public System.Version ProductVersion  = new System.Version(1, 1, 23);


        public bool IsLoaded { get; private set; }

        public AnimalManager AInstance
        {
            get
            {
                return (AnimalManager)typeof(AnimalManager).GetField("manager", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static).GetValue(null);
            }
        }
       

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
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

        protected override void Load()
        {
            Instance = this;
            U.Events.OnPlayerDisconnected += onDisconnected_Event;
            Level.onLevelLoaded += onLoaded_Event;
            Provider.onServerShutdown += onServerShutdown;
            var original10 = typeof(AnimalManager).GetMethod("askAnimalAttack");
            var prefix10 = typeof(AnimalAttackPatch).GetMethod("Prefix");
            Harmony.Patch(original10, new HarmonyMethod(prefix10), null, null);

            var original = typeof(Animal).GetMethod("askDamage");
            var prefix = typeof(AskDamagePatch).GetMethod("Prefix");
            Harmony.Patch(original, new HarmonyMethod(prefix), null, null);


            var original2 = typeof(Animal).GetMethod("tick");
            var prefix2 = typeof(AnimalTickPatch).GetMethod("Prefix");
            Harmony.Patch(original2, new HarmonyMethod(prefix2), null, null);
            U.Events.OnPlayerConnected += onConnected;

            IsLoaded = true;

        }


        private void onConnected(UnturnedPlayer player)
        {

        }

        private void onServerShutdown()
        {
            foreach (var item in PetHandler.PlayerPets)
            {
                item.Dispose();
            }
        }

        protected override void Unload()
        {
            if (!IsLoaded)
            {
                return;
            }

            Instance = null;
            U.Events.OnPlayerDisconnected -= onDisconnected_Event;
            Level.onLevelLoaded -= onLoaded_Event;
            Provider.onServerShutdown -= onServerShutdown;
            U.Events.OnPlayerConnected -= onConnected;
            IsLoaded = false;

        }


        public void FixedUpdate()
        {
            if (!IsLoaded)
                return;

            //I know this is supposed to be where I show my skills but I can't bother patching every single movment method to force it to only move to me so I'll just do a spam thingy.
            if (!Level.isLoaded)
                return;    

            if (DateTime.Now > _check.AddSeconds(0.8))
            {
                _check = DateTime.Now;
                for(int i = 0; i < PetHandler.PlayerPets.Count; i++)
                {
                    var item = PetHandler.PlayerPets[i];
                    if (item.Animal == null)
                        continue;

                    if (item.Player != null)
                    {
                        ReflectionUtil.setValue("_isFleeing", true, item.Animal);
                        ReflectionUtil.setValue("isWandering", false, item.Animal);
                        ReflectionUtil.setValue("isHunting", false, item.Animal);
                        ReflectionUtil.callMethod("updateTicking", item.Animal);
                        /*
                        item?.animal?.alertPlayer(uplayer?.Player, false);
                        ReflectionUtil.setValue("_isFleeing", false, item.animal);
                        ReflectionUtil.setValue("isWandering", false, item.animal);
                        ReflectionUtil.setValue("isHunting", true, item.animal);
                        */
                        if (Vector3.Distance(item.Animal.transform.position, item.Player.transform.position) > Configuration.Instance.MaxDistanceBetweenPetAndOwner)
                        {
                            item.Animal.transform.position = item.Player.transform.position;
                        }
                    }
                }
            }

        }

        public void sendAnimalDead(Animal animal)
        {
            StartCoroutine(killAnimalDelay(animal, 1f));
        }


        IEnumerator killAnimalDelay(Animal animal, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            AnimalManager.sendAnimalDead(animal, new Vector3(0, 0, 0));
        }

        public void sendAnimalRemove(Animal animal)
        {
            int index = AnimalManager.animals.IndexOf(animal);
            animal.transform.position = Vector3.zero;
            StartCoroutine(AnimalRemove(animal, index));
        }

        IEnumerator AnimalRemove(Animal animal, int index)
        {
            yield return new WaitForSeconds(1f);
            //Sets the position earlier
            AInstance.channel.send("tellAnimalDead", ESteamCall.OTHERS, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
            {
                index,
                animal.transform.position,
                (byte)ERagdollEffect.NONE
            });
            AnimalManager.tickingAnimals.Remove(animal);

            Rocket.Core.Logging.Logger.LogWarning("Killed animal because we're putting it o cacge");
        }

        public void sendAnimalMove(Animal animal, Vector3 pos)
        {
            StartCoroutine(AnimalMove(new object[] { animal, pos }, 1f));
        }


        IEnumerator AnimalMove(object[] args, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            Animal animal = (Animal)args[0];
            Vector3 pos = (Vector3)args[1];
            animal.transform.position = pos;
        }

        private void onLoaded_Event(int level)
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 200;
            timer.Enabled = true;
        }


        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            foreach (var item in PetHandler.PlayerPets)
            {
                if (DateTime.Now > item.SummonedAt.AddSeconds(item.Asset.MaxAliveTime))
                {
                    var trans = new Transelation(this, "max_time_reached");
                    trans.execute(UnturnedPlayer.FromPlayer(item.Player));
                    item.DespawnAnimal();
                }
            }
            return;
        }
        
        private void onDisconnected_Event(UnturnedPlayer player)
        {
            var item = PetHandler.PlayerPets.Find(c => (c.Player == player.Player));
            if (item != null)
            {
                item.Dispose();
                PetHandler.PlayerPets.Remove(item);
            }
        }
    }
}

