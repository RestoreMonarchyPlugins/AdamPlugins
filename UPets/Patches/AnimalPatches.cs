using Adam.PetsPlugin.Handlers;
using HarmonyLib;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UPets.Reflection;

namespace Adam.PetsPlugin.Patches
{
    [HarmonyPatch(typeof(Animal))]
    class AnimalPatches
    {
        [HarmonyPatch("tick")]
        [HarmonyPrefix]
        static bool tick_Prefix(Animal __instance)
        {
            var pet = PetsPlugin.Instance.PetsService.GetPet(__instance);
            if (pet == null)
                return true;

            Vector3 playerPos = pet.Player.transform.position;
            Vector3 playerDirection = pet.Player.transform.forward;
            Quaternion playerRotation = pet.Player.transform.rotation;
            float spawnDistance = 5;

            Vector3 spawnPos = playerPos + (((pet.Player.transform.right) + pet.Player.transform.forward) * spawnDistance);

            spawnPos.y = LevelGround.getHeight(spawnPos);

            float delta = Time.time - (float)ReflectionUtil.getValue("lastTick", __instance);
            ReflectionUtil.setValue("lastTick", Time.time, __instance);

            ReflectionUtil.setValue("target", spawnPos, __instance);
            ReflectionUtil.setValue("_isFleeing", true, __instance);
            ReflectionUtil.setValue("player", null, __instance);
            ReflectionUtil.setValue("isAttacking", false, __instance);
            ReflectionUtil.setValue("_isFleeing", true, pet.Animal);
            ReflectionUtil.setValue("isWandering", false, pet.Animal);
            ReflectionUtil.setValue("isHunting", false, pet.Animal);
            ReflectionUtil.callMethod("move", __instance, delta);
            return false;
        }

        [HarmonyPatch("askDamage")]
        [HarmonyPrefix]
        static bool askDamage_Prefix(Animal __instance)
        {
            return !PetsPlugin.Instance.PetsService.IsPet(__instance);
        }
    }
}
