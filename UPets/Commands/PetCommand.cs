using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using SDG.Unturned;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using UnityEngine;
using fr34kyn01535.Uconomy;
using Adam.PetsPlugin.Transelations;
using Adam.PetsPlugin.Handlers;
using Adam.PetsPlugin.Models;

namespace Adam.PetsPlugin
{
    public class SummonPetCommand : IRocketCommand
    {
        private PetsPlugin pluginInstance => PetsPlugin.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                HelpCommand(caller);
                return;
            }

            switch (command[0].ToLower())
            {
                case "list":
                    ListCommand(caller);
                    break;
                case "shop":
                    ShopCommand(caller);
                    break;
                default:
                    break;
            }
        }

        private void ShopCommand(IRocketPlayer caller)
        {
            StringBuilder sb = new StringBuilder(pluginInstance.Translate("PetShopAvailable"));
            foreach (var petConfig in pluginInstance.Configuration.Instance.Pets)
            {
                if (!string.IsNullOrEmpty(petConfig.RequiredPermission) &&  !caller.HasPermission(petConfig.RequiredPermission))
                {
                    continue;
                }
                sb.Append($" {petConfig.Name}[{petConfig.Cost}],");
            }

            if (sb.Length < 2)
                UnturnedChat.Say(caller, pluginInstance.Translate("PetShopNone"), pluginInstance.MessageColor);
            else
                UnturnedChat.Say(caller, pluginInstance.Translate("PetShopAvailable", sb.ToString().TrimEnd(',')), pluginInstance.MessageColor);
        }

        private void ListCommand(IRocketPlayer caller)
        {
            PetConfig petConfig;
            List<string> pets = new List<string>();
            foreach (var pet in pluginInstance.Database.GetPlayerPets(caller.Id))
            {
                petConfig = pluginInstance.Configuration.Instance.Pets.FirstOrDefault(x => x.Id == pet.Id);
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
            UnturnedChat.Say(caller, pluginInstance.Translate("HelpLine1"), pluginInstance.MessageColor);
            UnturnedChat.Say(caller, pluginInstance.Translate("HelpLine2"), pluginInstance.MessageColor);
            UnturnedChat.Say(caller, pluginInstance.Translate("HelpLine3"), pluginInstance.MessageColor);
            UnturnedChat.Say(caller, pluginInstance.Translate("HelpLine4"), pluginInstance.MessageColor);
        }


        public List<string> Aliases => new List<string>();

        public string Help => "Pet management command";

        public string Name => "pet";

        public List<string> Permissions => new List<string>();

        public string Syntax => "<option>";

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
    }
}
