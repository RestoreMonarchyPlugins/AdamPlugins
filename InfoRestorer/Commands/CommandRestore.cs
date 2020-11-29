using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Adam.InfoRestorer.Commands
{
    public class CommandRestore : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "restore";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { Name };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            //restore <player> <timesAgo>
            if(command.Length < 2)
            {
                UnturnedChat.Say(caller, InfoRestorerPlugin.Instance.Translate("invalid_syntax"), Color.red);
                return;
            }
            var found = UnturnedPlayer.FromName(command[0]);
            if(found == null)
            {
                UnturnedChat.Say(caller, InfoRestorerPlugin.Instance.Translate("player_not_found"), Color.red);
                return;
            }

            uint timesAgo = 0;
            if(!uint.TryParse(command[1], out timesAgo) || timesAgo == 0)
            {
                UnturnedChat.Say(caller, InfoRestorerPlugin.Instance.Translate("not_number", command[1]), Color.red);
                return;
            }

            var otherSession = InfoRestorerPlugin.Instance.GetSession(found);
            long index = otherSession.Slots.Count - timesAgo;
            if(index < 0 || index >= otherSession.Slots.Count)
            {
                UnturnedChat.Say(caller, InfoRestorerPlugin.Instance.Translate("too_much", found.CharacterName), Color.red);
                return;
            }
            var slot = otherSession.Slots[(int)index];
            slot.Load(InfoRestorerPlugin.Instance.Configuration.Instance.ShouldClearInventory);
            UnturnedChat.Say(caller, InfoRestorerPlugin.Instance.Translate("restored", found.CharacterName), Color.yellow);
        }
    }
}
