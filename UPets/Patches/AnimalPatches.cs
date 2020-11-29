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
            var item = PetHandler.PlayerPets.Find(c => (c.Animal == __instance));
            if (item == null || __instance.isDead)
                return true;

            Vector3 playerPos = item.Player.transform.position;
            Vector3 playerDirection = item.Player.transform.forward;
            Quaternion playerRotation = item.Player.transform.rotation;
            float spawnDistance = 5;

            Vector3 spawnPos = playerPos + (((item.Player.transform.right) + item.Player.transform.forward) * spawnDistance);

            spawnPos.y = LevelGround.getHeight(spawnPos);

            float delta = Time.time - (float)ReflectionUtil.getValue("lastTick", __instance);
            ReflectionUtil.setValue("lastTick", Time.time, __instance);

            ReflectionUtil.setValue("target", spawnPos, __instance);
            ReflectionUtil.setValue("_isFleeing", true, __instance);
            ReflectionUtil.setValue("player", null, __instance);
            ReflectionUtil.setValue("isAttacking", false, __instance);
            ReflectionUtil.setValue("_isFleeing", true, item.Animal);
            ReflectionUtil.setValue("isWandering", false, item.Animal);
            ReflectionUtil.setValue("isHunting", false, item.Animal);
            ReflectionUtil.callMethod("move", __instance, delta);
            return false;
        }

        [HarmonyPatch("askDamage")]
        [HarmonyPrefix]
        static bool askDamage_Prefix(SDG.Unturned.Animal __instance, byte amount, Vector3 newRagdoll, out EPlayerKill kill, out uint xp, bool trackKill)
        {
            xp = 0;
            kill = EPlayerKill.NONE;
            if (PetHandler.PlayerPets.Find(c => (c.Animal == __instance)) != null)
                return false;
            return true;
        }
    }
}
