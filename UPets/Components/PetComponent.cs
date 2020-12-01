using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UPets.Reflection;

namespace Adam.PetsPlugin.Components
{
    public class PetComponent : MonoBehaviour
    {
        public Animal Animal { get; private set; }

        void Awake()
        {
            Animal = GetComponent<Animal>();
        }

        void Start()
        {

        }

        void OnDestroy()
        {

        }

        public void MovePet(Vector3 position)
        {
            ReflectionUtil.setValue("_isFleeing", true, Animal);
            ReflectionUtil.setValue("isWandering", false, Animal);
            ReflectionUtil.setValue("isHunting", false, Animal);
            ReflectionUtil.callMethod("updateTicking", Animal);

            Animal.transform.position = position;
        }
    }
}
