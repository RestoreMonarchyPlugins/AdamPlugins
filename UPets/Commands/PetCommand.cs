﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Adam.PetsPlugin.Models;
using Adam.PetsPlugin.Helpers;
using System.Threading;
using Rocket.Core.Utils;
using Rocket.Core.Logging;

namespace Adam.PetsPlugin
{
    public class PetCommand : IRocketCommand
    {
        private PetsPlugin pluginInstance => PetsPlugin.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                HelpCommand(caller);
                return;
            }

            string option = command[0].ToLower();
            if (option == "list")
            {
                RunAsync(() => ListCommand(caller));
            }
            else if (option == "shop")
            {
                ShopCommand(caller);
            }
            else if (option == "help")
            {
                HelpCommand(caller);
            }
            else if (option == "buy")
            {
                if (TryGetPetConfig(caller, command.ElementAtOrDefault(1), out PetConfig config))
                {
                    RunAsync(() => BuyCommand((UnturnedPlayer)caller, config));
                }
            }
            else
            {
                if (TryGetPetConfig(caller, command[0], out PetConfig config))
                {
                    SpawnCommand((UnturnedPlayer)caller, config);
                }
            }
        }

        private void RunAsync(Action action)
        {
            ThreadPool.QueueUserWorkItem((_) => 
            { 
                try
                {
                    action.Invoke();
                } catch (Exception e)
                {
                    TaskDispatcher.QueueOnMainThread(() => Logger.LogException(e, $"YA'LL WANNA SINGLE SAY"));
                }
            });
        }

        private bool TryGetPetConfig(IRocketPlayer caller, string value, out PetConfig config)
        {
            config = null;
            if (value == null)
            {
                pluginInstance.ReplyPlayer(caller, "PetNameRequired", value);
                return false;
            }

            config = pluginInstance.Configuration.Instance.Pets.FirstOrDefault(x => x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (config == null)
            {
                pluginInstance.ReplyPlayer(caller, "PetNotFound", value);
                return false;   
            }
            return true;
        }

        public void SpawnCommand(UnturnedPlayer player, PetConfig config)
        {
            var pets = pluginInstance.PetsService.GetPlayerActivePets(player.Id);
            PlayerPet pet;
            if (pets != null && pets.Count() > 0)
            {
                pet = pets.First();

                pluginInstance.PetsService.KillPet(pet);

                if (pet.AnimalId == config.Id)
                {
                    pluginInstance.ReplyPlayer(player, "PetDespawnSuccess", config.Name);
                    return;
                }
            }

            RunAsync(() => 
            {
                pet = pluginInstance.Database.GetPlayerPets(player.Id).FirstOrDefault(x => x.AnimalId == config.Id);
                if (pet != null)
                {
                    TaskDispatcher.QueueOnMainThread(() => pluginInstance.PetsService.SpawnPet(player, pet));
                    pluginInstance.ReplyPlayer(player, "PetSpawnSuccess", config.Name);
                }
                else
                    pluginInstance.ReplyPlayer(player, "PetSpawnFail", config.Name);
            });            
        }

        public void BuyCommand(UnturnedPlayer player, PetConfig config)
        {
            if (!player.HasPermission(config.Permission) && !player.IsAdmin && !string.IsNullOrEmpty(config.Permission))
            {
                pluginInstance.ReplyPlayer(player, "PetBuyNoPermission", config.Name);
                return;
            }

            if (pluginInstance.Database.GetPlayerPets(player.Id).Any(x => x.AnimalId == config.Id))
            {
                pluginInstance.ReplyPlayer(player, "PetBuyAlreadyHave", config.Name);
                return;
            }

            if (UconomyHelper.GetPlayerBalance(player.Id) < config.Cost)
            {
                pluginInstance.ReplyPlayer(player, "PetCantAfford", config.Name, config.Cost);
                return;
            }

            UconomyHelper.IncreaseBalance(player.Id, config.Cost * -1);
            pluginInstance.Database.AddPlayerPet(new PlayerPet()
            {
                AnimalId = config.Id,
                PlayerId = player.Id,
                PurchaseDate = DateTime.UtcNow
            });
            pluginInstance.ReplyPlayer(player, "PetBuySuccess", config.Name, config.Cost);
        }

        private void ShopCommand(IRocketPlayer caller)
        {
            StringBuilder sb = new StringBuilder("PetShopAvailable");
            foreach (var petConfig in pluginInstance.Configuration.Instance.Pets)
            {
                if (string.IsNullOrEmpty(petConfig.Permission) || caller.IsAdmin || caller.HasPermission(petConfig.Permission))
                {
                    sb.Append($" {petConfig.Name}[{petConfig.Cost}],");
                }                
            }

            if (sb.Length < 2)
                pluginInstance.ReplyPlayer(caller, "PetShopNone");
            else
                pluginInstance.ReplyPlayer(caller, sb.ToString().TrimEnd(','));
        }

        private void ListCommand(IRocketPlayer caller)
        {
            PetConfig petConfig;
            List<string> pets = new List<string>();
            foreach (var pet in pluginInstance.Database.GetPlayerPets(caller.Id))
            {
                petConfig = pluginInstance.Configuration.Instance.Pets.FirstOrDefault(x => x.Id == pet.AnimalId);
                if (petConfig != null)
                {
                    pets.Add(petConfig.Name);
                }                
            }

            if (pets.Count == 0)
                pluginInstance.ReplyPlayer(caller, "PetListNone");
            else
                pluginInstance.ReplyPlayer(caller, "PetList", string.Join(", ", pets));
            
        }

        private void HelpCommand(IRocketPlayer caller)
        {
            pluginInstance.ReplyPlayer(caller, "PetHelpLine1");
            pluginInstance.ReplyPlayer(caller, "PetHelpLine2");
            pluginInstance.ReplyPlayer(caller, "PetHelpLine3");
            pluginInstance.ReplyPlayer(caller, "PetHelpLine4");
        }


        public List<string> Aliases => new List<string>();

        public string Help => "Pet management command";

        public string Name => "pet";

        public List<string> Permissions => new List<string>();

        public string Syntax => "<option>";

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
    }
}
