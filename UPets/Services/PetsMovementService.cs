using Adam.PetsPlugin.Handlers;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Adam.PetsPlugin.Services
{
    public class PetsMovementService : MonoBehaviour
    {
        private PetsPlugin pluginInstance => PetsPlugin.Instance;

        void Awake()
        {

        }

        void Start()
        {
            UnturnedPlayerEvents.OnPlayerUpdatePosition += FollowPlayer;
        }

        void OnDestroy()
        {
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= FollowPlayer;
        }

        private void FollowPlayer(UnturnedPlayer player, Vector3 position)
        {

            for (int i = 0; i < PetHandler.PlayerPets.Count; i++)
            {
                var item = PetHandler.PlayerPets[i];
                if (item.Animal == null)
                    continue;

                if (item.Player != null)
                {
                    
                }
            }
        }
    }
}
