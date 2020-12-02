using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Adam.PetsPlugin.Models;
using Adam.PetsPlugin.Helpers;

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
                ListCommand(caller);
            } else if (option == "shop")
            {
                ShopCommand(caller);
            } else if (option == "buy")
            {
                if (TryGetPetConfig(caller, command.ElementAtOrDefault(1), out PetConfig config))
                {
                    BuyCommand((UnturnedPlayer)caller, config);
                }
            } else
            {
                if (TryGetPetConfig(caller, command[0], out PetConfig config))
                {
                    SpawnCommand((UnturnedPlayer)caller, config);
                }
            }
        }

        private bool TryGetPetConfig(IRocketPlayer caller, string value, out PetConfig config)
        {
            config = null;
            if (value == null)
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("PetNameRequired"), pluginInstance.MessageColor);
                return false;
            }

            config = pluginInstance.Configuration.Instance.Pets.FirstOrDefault(x => x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (config == null)
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("PetNotFound", value), pluginInstance.MessageColor);
                return false;
            }
            return true;
        }

        public void SpawnCommand(UnturnedPlayer player, PetConfig config)
        {
            var pets = pluginInstance.PetsService.GetPlayerActivePets(player.Id);
            if (pets != null && pets.Count() > 0)
            {
                var pet = pets.First();
                pluginInstance.PetsService.KillPet(pet);

                if (pet.AnimalId == config.Id)
                {
                    UnturnedChat.Say(player, pluginInstance.Translate("PetDespawnSuccess", config.Name), pluginInstance.MessageColor);
                    return;
                }
            }

            if (pluginInstance.PetsService.TrySpawnPet(player, config.Id))
            {
                UnturnedChat.Say(player, pluginInstance.Translate("PetSpawnSuccess", config.Name), pluginInstance.MessageColor);
            } else
            {
                UnturnedChat.Say(player, pluginInstance.Translate("PetSpawnFail", config.Name), pluginInstance.MessageColor);
            }
        }

        public void BuyCommand(UnturnedPlayer player, PetConfig config)
        {
            if (pluginInstance.Database.GetPlayerPets(player.Id).Any(x => x.AnimalId == config.Id))
            {
                UnturnedChat.Say(player, pluginInstance.Translate("PetBuyAlreadyHave"), pluginInstance.MessageColor);
                return;
            }

            if (UconomyHelper.GetPlayerBalance(player.Id) < config.Cost)
            {
                UnturnedChat.Say(player, pluginInstance.Translate("PetCantAfford", config.Name, config.Cost), pluginInstance.MessageColor);
                return;
            }

            UconomyHelper.IncreaseBalance(player.Id, config.Cost * -1);
            pluginInstance.Database.AddPlayerPet(new PlayerPet()
            {
                AnimalId = config.Id,
                PlayerId = player.Id,
                PurchaseDate = DateTime.UtcNow
            });
            UnturnedChat.Say(player, pluginInstance.Translate("PetBuySuccess", config.Name, config.Cost), pluginInstance.MessageColor);
        }

        private void ShopCommand(IRocketPlayer caller)
        {
            StringBuilder sb = new StringBuilder(pluginInstance.Translate("PetShopAvailable"));
            foreach (var petConfig in pluginInstance.Configuration.Instance.Pets)
            {
                if (string.IsNullOrEmpty(petConfig.Permission) || caller.IsAdmin || caller.HasPermission(petConfig.Permission))
                {
                    sb.Append($" {petConfig.Name}[{petConfig.Cost}],");
                }                
            }

            if (sb.Length < 2)
                UnturnedChat.Say(caller, pluginInstance.Translate("PetShopNone"), pluginInstance.MessageColor);
            else
                UnturnedChat.Say(caller, sb.ToString().TrimEnd(','), pluginInstance.MessageColor);
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
                UnturnedChat.Say(caller, pluginInstance.Translate("PetListNone"), pluginInstance.MessageColor);
            else
                UnturnedChat.Say(caller, pluginInstance.Translate("PetList", string.Join(", ", pets)), pluginInstance.MessageColor);
            
        }

        private void HelpCommand(IRocketPlayer caller)
        {
            UnturnedChat.Say(caller, pluginInstance.Translate("PetHelpLine1"), pluginInstance.MessageColor);
            UnturnedChat.Say(caller, pluginInstance.Translate("PetHelpLine2"), pluginInstance.MessageColor);
            UnturnedChat.Say(caller, pluginInstance.Translate("PetHelpLine3"), pluginInstance.MessageColor);
            UnturnedChat.Say(caller, pluginInstance.Translate("PetHelpLine4"), pluginInstance.MessageColor);
        }


        public List<string> Aliases => new List<string>();

        public string Help => "Pet management command";

        public string Name => "pet";

        public List<string> Permissions => new List<string>();

        public string Syntax => "<option>";

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
    }
}
