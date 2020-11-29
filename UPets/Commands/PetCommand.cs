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
        public List<string> Aliases => new List<string>();

        public string Help => "Pet management command";

        public string Name => "pet";

        public List<string> Permissions => new List<string>();

        public string Syntax => "<option>";

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
                PetHandler.spawnPet(player, Plugin.Instance.Configuration.Instance.Pets.Find(c => (c.Id == item.lastPetUsed)));
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
                        .OrderBy(a => a.Name.Length)
                        .FirstOrDefault(a => a.Name.ToLower().Contains(command[1].ToLower()));

                    if (w == null)
                    {
                        var trans = new Transelation(Plugin.Instance, "cant_find_global_pet", command[1]);
                        trans.execute(player);
                        return;
                    }

                    var playerItem = DataHandler.getPlayerD((ulong)player.CSteamID);
                    if (playerItem != null && playerItem.pets.Contains(w.Id) || w.IfHasPermissionGetForFree && player.HasPermission(w.RequiredPermission))
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

                    if (pbal < w.Cost)
                    {
                        var trans = new Transelation(Plugin.Instance, "cant_afford", w.Cost);
                        trans.execute(player);
                        return;
                    }



                    if (Plugin.IsDependencyLoaded("Uconomy"))
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -w.Cost);
                    }
                    else
                    {
                        Rocket.Core.Logging.Logger.LogWarning("Error - No economy plugin found! Please install Uconomy or AviEconomy!");
                        return;
                    }

                    DataHandler.addPetToList((ulong)player.CSteamID, w.Id);
                    var trans5 = new Transelation(Plugin.Instance, "succesfully_bought", w.Name, w.Cost);
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
                        .OrderBy(a => a.Name.Length)
                        .FirstOrDefault(a => a.Name.ToLower().Contains(command[0].ToLower()) 
                        && a.HasPet((ulong)player.CSteamID)
                        || a.Name.ToLower().Contains(command[0].ToLower()) 
                        && a.IfHasPermissionGetForFree
                        && player.HasPermission(a.RequiredPermission)
                        );



                    if (asset == null)
                    {
                        var trans = new Transelation(Plugin.Instance, "cant_find_pet", command[0]);
                        trans.execute(player);
                        return;
                    }
                    PetHandler.spawnPet(player, asset);
                    var trans2 = new Transelation(Plugin.Instance, "succesfully_spawned_pet");
                    DataHandler.setLatestPet((ulong)player.CSteamID, asset.Id);
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
                this.name = asset.Name;
                this.cost = asset.Cost;
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
