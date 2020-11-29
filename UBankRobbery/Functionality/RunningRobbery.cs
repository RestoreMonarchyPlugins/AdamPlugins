using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBankRobbery.Regions;
using UnityEngine;

namespace UBankRobbery.Functionality
{
    public class RunningRobbery
    {
        public RunningRobbery(Player robber, BankRobberyRegionConfiguration region)
        {
            this.Robber = robber ?? throw new ArgumentNullException(nameof(robber));
            this.Region = region ?? throw new ArgumentNullException(nameof(region));
            StartedAtUtc = DateTime.UtcNow;
        }
        public Player Robber { get; }
        public BankRobberyRegionConfiguration Region { get; }
        public DateTime StartedAtUtc { get; }

        public bool IsCompleted => (DateTime.UtcNow - StartedAtUtc).TotalSeconds > Region.RobbingDuration;

        public void IssueReward()
        {
            var random = new System.Random();
            var reward = random.Next(Region.MinimumReward, Region.MaximumReward);
            var uPlayer = UnturnedPlayer.FromPlayer(Robber);
            uPlayer.Experience += (uint)reward;
            UnturnedChat.Say(Plugin.Instance.Translate("finished", uPlayer.CharacterName), Color.yellow);
        }


    }
}
