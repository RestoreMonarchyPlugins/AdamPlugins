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

namespace Adam.PetsPlugin
{
    public class SummonPetCommand : IRocketCommand
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>();
            }
        }

        public string Help
        {
            get
            {
                return "Summons a pet!";
            }
        }

        public string Name
        {
            get
            {
                return "pet";
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "pet"
                };
            }
        }

        public string Syntax
        {
            get
            {
                return "/pet <name>";
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if(command.Length == 0)
            {
                #region noLastPet
                var item = DataHandler.getPlayerD((ulong)player.CSteamID);
                if (item == null || item.lastPetUsed == 0)
                {
                    var trans = new Transelation(Plugin.Instance, "no_lastpet");
                    trans.execute(player);
                    return;
                }
                PetHandler.spawnPet(player, Plugin.Instance.Configuration.Instance.Pets.Find(c => (c.id == item.lastPetUsed)));
                var trans2 = new Transelation(Plugin.Instance, "succesfully_spawned_pet");
                trans2.execute(player);
                return;
                #endregion
            }

            switch (command[0].ToLower())
            {
                #region buy
                case "buy":

                    if (command.Length < 2)
                    {
                        var trans = new Transelation(Plugin.Instance, "invalid_syntax");
                        trans.execute(player);
                        return;
                    }

                    var w = Plugin.Instance.Configuration.Instance.Pets.Where(a => a != null)
                        .OrderBy(a => a.name.Length)
                        .FirstOrDefault(a => a.name.ToLower().Contains(command[1].ToLower()));

                    if (w == null)
                    {
                        var trans = new Transelation(Plugin.Instance, "cant_find_global_pet", command[1]);
                        trans.execute(player);
                        return;
                    }

                    var playerItem = DataHandler.getPlayerD((ulong)player.CSteamID);
                    if (playerItem != null && playerItem.pets.Contains(w.id) || w.ifHasPermissionGetForFree && player.HasPermission(w.requiredPermission))
                    {
                        var trans = new Transelation(Plugin.Instance, "already_own");
                        trans.execute(player);
                        return;
                    }
                    decimal pbal = 0;

                    if (Plugin.IsDependencyLoaded("Uconomy"))
                    {
                        
                        pbal = Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString());
                    }
                    else
                    {
                        Rocket.Core.Logging.Logger.LogWarning("Error - No economy plugin found! Please install Uconomy or AviEconomy!");
                        return;
                    }

                    if (pbal < w.cost)
                    {
                        var trans = new Transelation(Plugin.Instance, "cant_afford", w.cost);
                        trans.execute(player);
                        return;
                    }



                    if (Plugin.IsDependencyLoaded("Uconomy"))
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -w.cost);
                    }
                    else
                    {
                        Rocket.Core.Logging.Logger.LogWarning("Error - No economy plugin found! Please install Uconomy or AviEconomy!");
                        return;
                    }

                    DataHandler.addPetToList((ulong)player.CSteamID, w.id);
                    var trans5 = new Transelation(Plugin.Instance, "succesfully_bought", w.name, w.cost);
                    trans5.execute(player);
                    break;
                #endregion
                #region list
                case "list":
                    var pets = Plugin.Instance.Configuration.Instance.Pets.ToArray().Select(item => new infoItem(item));

                    var print = string.Join(", ", pets.Select(item => (string)item).ToArray());

                    var trans1 = new Transelation(Plugin.Instance, "all_pets", print);
                    trans1.execute(player);
                    break;
                #endregion
                #region despawn
                case "despawn":
                    var rs = PetHandler.despawnpet(player);
                    if (rs)
                    {
                        var trans = new Transelation(Plugin.Instance, "succesfully_despawned");
                        trans.execute(player);
                        return;
                    }
                    else
                    {
                        var trans = new Transelation(Plugin.Instance, "using_no_pet");
                        trans.execute(player);
                        return;
                    }
                #endregion
                #region help
                case "help":
                    Transelation transelationHelp = new Transelation(Plugin.Instance, "help_line");
                    transelationHelp.execute(player);
                    return;
                #endregion
                #region spawnPet
                default:
                    var asset = Plugin.Instance.Configuration.Instance.Pets.
                        Where(a => a != null)
                        .OrderBy(a => a.name.Length)
                        .FirstOrDefault(a => a.name.ToLower().Contains(command[0].ToLower()) 
                        && a.HasPet((ulong)player.CSteamID)
                        || a.name.ToLower().Contains(command[0].ToLower()) 
                        && a.ifHasPermissionGetForFree
                        && player.HasPermission(a.requiredPermission)
                        );



                    if (asset == null)
                    {
                        var trans = new Transelation(Plugin.Instance, "cant_find_pet", command[0]);
                        trans.execute(player);
                        return;
                    }
                    PetHandler.spawnPet(player, asset);
                    var trans2 = new Transelation(Plugin.Instance, "succesfully_spawned_pet");
                    DataHandler.setLatestPet((ulong)player.CSteamID, asset.id);
                    trans2.execute(player);
                    break;
            }
                #endregion
            


        }

        public class infoItem
        {
            public string name;
            public decimal cost;

            public infoItem(PetAsset asset)
            {
                this.name = asset.name;
                this.cost = asset.cost;
            }
            public infoItem() { }

            public static explicit operator string(infoItem item)
            {

                return item.name + " | " + item.cost;
            }

            public override string ToString()
            {
                return name + " | " + cost;
            }
        }
    }
}
