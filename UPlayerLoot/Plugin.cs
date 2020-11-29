using HarmonyLib;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using UnityEngine;
using UPlayerLoot.Extensions;
using UPlayerLoot.Serialization;

namespace UPlayerLoot
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance { get; private set; }

        public const string HarmonyInstanceId = "de.uplayerloot";
        private Harmony HarmonyInstance { get; set; }

        public PlayerInfoSerializer InfoSerializer { get; private set; }

        protected override void Load()
        {            
            Instance = this;
            HarmonyInstance = new Harmony(HarmonyInstanceId);
            HarmonyInstance.PatchAll(Assembly);
            InfoSerializer = new PlayerInfoSerializer(Directory, "{0}.json");

            Level.onLevelLoaded += OnLevelLoaded;
            Provider.onServerShutdown += Shutdown;
            U.Events.OnPlayerDisconnected += Disconnected;
            BarricadeManager.onDamageBarricadeRequested += BarricadeDamaged;

            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {            
            Level.onLevelLoaded -= OnLevelLoaded;
            Provider.onServerShutdown -= Shutdown;
            U.Events.OnPlayerDisconnected -= Disconnected;
            BarricadeManager.onDamageBarricadeRequested -= BarricadeDamaged;

            HarmonyInstance.UnpatchAll(HarmonyInstanceId);
            
            Rocket.Core.Logging.Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        private void Shutdown()
        {
            foreach(var player in Provider.clients.Select(c => UnturnedPlayer.FromSteamPlayer(c)))
            {
                var playerInfo = InfoSerializer.Players.FirstOrDefault(c => c.Id == player.CSteamID.m_SteamID);
                if (playerInfo == null)
                {
                    playerInfo = new PlayerInfo()
                    {
                        Id = player.CSteamID.m_SteamID
                    };
                    InfoSerializer.Players.Add(playerInfo);
                }
                AddCharacter(player, playerInfo);
            }
            InfoSerializer.Save();
        }

        private void BarricadeDamaged(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!shouldAllow 
                || !BarricadeManager.tryGetInfo(barricadeTransform, out byte x, out byte y, out ushort plant, out ushort index, out var region, out var drop) 
                || plant != ushort.MaxValue)
                return;

            var newHp = region.barricades.FirstOrDefault(c => c.instanceID == drop.instanceID).barricade.health - pendingTotalDamage;

            if (newHp > 0)
                return;
            var barricade = region.barricades.FirstOrDefault(c => c.instanceID == drop.instanceID);

            if (UnturnedPlayer.FromCSteamID((CSteamID)barricade.owner)?.Player != null)
                return;

            var player = PlayerTool.getPlayer(instigatorSteamID);

            var playerInfo = InfoSerializer.Players.FirstOrDefault(c => c.Characters.Any(k => k.InstanceId == drop.instanceID));

            var character = playerInfo?.Characters?.FirstOrDefault(c => c.InstanceId == drop.instanceID);

            if (character == null)
                return;


            foreach (var item in character.Items)
            {
                ItemManager.dropItem(new Item(item.ItemId, item.Amount, item.Quality, item.State), character.BarricadeLocation.ToVector3() + new Vector3(0, 0.5f), true, true, true);
            }

            playerInfo.Characters.Remove(character);

            InfoSerializer.Save();
        }

        private void OnLevelLoaded(int level)
        {
            InfoSerializer.Load();
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "DESTROYED", "Your storage container has been destroyed so you've lost your inventory!" }
        };

        private void Disconnected(UnturnedPlayer player)
        {
            var playerInfo = InfoSerializer.Players.FirstOrDefault(c => c.Id == player.CSteamID.m_SteamID);
            if (playerInfo == null)
            {
                playerInfo = new PlayerInfo()
                {
                    Id = player.CSteamID.m_SteamID
                };
                InfoSerializer.Players.Add(playerInfo);
            }
            AddCharacter(player, playerInfo);
            InfoSerializer.Save();
        }

        private void AddCharacter(UnturnedPlayer player, PlayerInfo playerInfo)
        {
            var items = player.Inventory.GetItems();

            if (Configuration.Instance.ClearClothing)
                items = items.Concat(player.Player.clothing.GetClothing());
            var pos = player.Position + (player.Player.transform.forward * 2f);
            pos += (player.Player.transform.up) * 1f;
            var character = new PlayerCharacter()
            {
                BarricadeLocation = new Vector3Wrapper(pos),
                Items = items.ToList(),
                CharacterId = player.Player.channel.owner.playerID.characterID
            };
            character.SummonContainer(player.Player.clothing, Configuration.Instance.MannequinId);

            playerInfo.Characters.Add(character);
        }
    }
}
