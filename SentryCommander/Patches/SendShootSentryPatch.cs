using HarmonyLib;
using Rocket.API;
using SDG.Framework.Utilities;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SentryCommander.Patches
{
    [HarmonyPatch(typeof(InteractableSentry), "Update")]
    internal static class SendShootSentryPatch
    {

        private const string IgnoreAmmoPermission = "sentrycommander.ammo";
        private const string GodPermission = "sentrycommander.god";

        private static FieldInfo hasWeaponField = typeof(InteractableSentry).GetField("hasWeapon", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo lastScanField = typeof(InteractableSentry).GetField("lastScan", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo lastFireField = typeof(InteractableSentry).GetField("lastFire", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo lastAimField = typeof(InteractableSentry).GetField("lastAim", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo fireTimeField = typeof(InteractableSentry).GetField("fireTime", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo attachmentsField = typeof(InteractableSentry).GetField("attachments", BindingFlags.NonPublic | BindingFlags.Instance);


        private static FieldInfo isFiringField = typeof(InteractableSentry).GetField("isFiring", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo isAimingField = typeof(InteractableSentry).GetField("isAiming", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo interactField = typeof(InteractableSentry).GetField("interact", BindingFlags.NonPublic | BindingFlags.Instance);


        private static FieldInfo displayAssetField = typeof(InteractableSentry).GetField("displayAsset", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo playersInRadiusField = typeof(InteractableSentry).GetField("playersInRadius", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo zombiesInRadiusField = typeof(InteractableSentry).GetField("zombiesInRadius", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo animalsInRadiusField = typeof(InteractableSentry).GetField("animalsInRadius", BindingFlags.NonPublic | BindingFlags.Static);

        private static FieldInfo aimTransformField = typeof(InteractableSentry).GetField("aimTransform", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo targetPlayerField = typeof(InteractableSentry).GetField("targetPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo targetZombieField = typeof(InteractableSentry).GetField("targetZombie", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo targetAnimalField = typeof(InteractableSentry).GetField("targetAnimal", BindingFlags.NonPublic | BindingFlags.Instance);


        [HarmonyPrefix]
        public static bool Prefix(InteractableSentry __instance)
        {

            RocketPlayer ownerPlayer = new RocketPlayer(__instance.owner.ToString());

            bool hasWeapon = (bool)hasWeaponField.GetValue(__instance);
            bool isFiring = (bool)isFiringField.GetValue(__instance);
            bool isAiming = (bool)isAimingField.GetValue(__instance);
            float lastScan = (float)lastScanField.GetValue(__instance);
            float lastFire = (float)lastFireField.GetValue(__instance);
            float fireTime = (float)fireTimeField.GetValue(__instance);
            float lastAim = (float)lastAimField.GetValue(__instance);
            ItemWeaponAsset displayAsset = displayAssetField.GetValue(__instance) as ItemWeaponAsset;
            Attachments attachments = attachmentsField.GetValue(__instance) as Attachments;

            bool interact = (bool)interactField.GetValue(__instance);

            var playersInRadius = (List<Player>)playersInRadiusField.GetValue(null);
            var zombiesInRadius = (List<Zombie>)zombiesInRadiusField.GetValue(null);
            var animalsInRadius = (List<Animal>)animalsInRadiusField.GetValue(null);

            var targetPlayer = targetPlayerField.GetValue(__instance) as Player;
            var targetZombie = targetZombieField.GetValue(__instance) as Zombie;
            var targetAnimal = targetAnimalField.GetValue(__instance) as Animal;
            var aimTransform = aimTransformField.GetValue(__instance) as Transform;
            if (__instance.isPowered)
            {
                Vector3 vector3_1 = __instance.transform.position + new Vector3(0.0f, 0.65f, 0.0f);
                Vector3 vector3_2;
                if ((double)Time.realtimeSinceStartup - (double)lastScan > 0.100000001490116)
                {
                    lastScanField.SetValue(__instance, Time.realtimeSinceStartup);

                    float a = 48f;
                    if (hasWeapon)
                        a = Mathf.Min(a, ((ItemWeaponAsset)displayAsset).range);
                    float sqrRadius = a * a;
                    float num = sqrRadius;
                    Player player = (Player)null;
                    Zombie zombie = (Zombie)null;
                    Animal animal = (Animal)null;
                    if (Provider.isPvP)
                    {
                        playersInRadius.Clear();
                        PlayerTool.getPlayersInRadius(vector3_1, sqrRadius, playersInRadius);
                        for (int index = 0; index < playersInRadius.Count; ++index)
                        {
                            Player playersInRadiu = playersInRadius[index];

                            var currentRocketPlayer = new RocketPlayer(playersInRadiu.channel.owner.playerID.steamID.ToString());

                            if (currentRocketPlayer.HasPermission(GodPermission))
                                continue;

                            if (!(playersInRadiu.channel.owner.playerID.steamID == __instance.owner) && !playersInRadiu.quests.isMemberOfGroup(__instance.group) && (!playersInRadiu.life.isDead && playersInRadiu.animator.gesture != EPlayerGesture.ARREST_START) && ((!playersInRadiu.movement.isSafe || !playersInRadiu.movement.isSafeInfo.noWeapons) && playersInRadiu.movement.canAddSimulationResultsToUpdates) && (!((UnityEngine.Object)player != (UnityEngine.Object)null) || playersInRadiu.animator.gesture != EPlayerGesture.SURRENDER_START) && (__instance.sentryMode != ESentryMode.FRIENDLY || (double)Time.realtimeSinceStartup - (double)playersInRadiu.equipment.lastPunching < 2.0 || playersInRadiu.equipment.isSelected && playersInRadiu.equipment.asset != null && playersInRadiu.equipment.asset.shouldFriendlySentryTargetUser))
                            {
                                vector3_2 = playersInRadiu.look.aim.position - vector3_1;
                                float sqrMagnitude = vector3_2.sqrMagnitude;
                                if ((double)sqrMagnitude <= (double)num)
                                {
                                    Vector3 vector3_3 = playersInRadiu.look.aim.position - vector3_1;
                                    float magnitude = vector3_3.magnitude;
                                    Vector3 vector3_4 = vector3_3 / magnitude;
                                    if (!((UnityEngine.Object)playersInRadiu != (UnityEngine.Object)targetPlayer) || (double)Vector3.Dot(vector3_4, aimTransform.forward) >= 0.5)
                                    {
                                        if ((double)magnitude > 0.025000000372529)
                                        {
                                            RaycastHit hit;
                                            PhysicsUtility.raycast(new Ray(vector3_1, vector3_4), out hit, magnitude - 0.025f, RayMasks.BLOCK_SENTRY, QueryTriggerInteraction.UseGlobal);
                                            if (!((UnityEngine.Object)hit.transform != (UnityEngine.Object)null) || !((UnityEngine.Object)hit.transform != (UnityEngine.Object)__instance.transform))
                                            {
                                                PhysicsUtility.raycast(new Ray(vector3_1 + vector3_4 * (magnitude - 0.025f), -vector3_4), out hit, magnitude - 0.025f, RayMasks.DAMAGE_SERVER, QueryTriggerInteraction.UseGlobal);
                                                if ((UnityEngine.Object)hit.transform != (UnityEngine.Object)null && (UnityEngine.Object)hit.transform != (UnityEngine.Object)__instance.transform)
                                                    continue;
                                            }
                                            else
                                                continue;
                                        }
                                        num = sqrMagnitude;
                                        player = playersInRadiu;
                                    }
                                }
                            }
                        }
                    }
                    zombiesInRadius.Clear();
                    ZombieManager.getZombiesInRadius(vector3_1, sqrRadius, zombiesInRadius);
                    for (int index = 0; index < zombiesInRadius.Count; ++index)
                    {
                        Zombie zombiesInRadiu = zombiesInRadius[index];
                        if (!zombiesInRadiu.isDead && zombiesInRadiu.isHunting)
                        {
                            Vector3 position = zombiesInRadiu.transform.position;
                            switch (zombiesInRadiu.speciality)
                            {
                                case EZombieSpeciality.NORMAL:
                                    position += new Vector3(0.0f, 1.75f, 0.0f);
                                    break;
                                case EZombieSpeciality.MEGA:
                                    position += new Vector3(0.0f, 2.625f, 0.0f);
                                    break;
                                case EZombieSpeciality.CRAWLER:
                                    position += new Vector3(0.0f, 0.25f, 0.0f);
                                    break;
                                case EZombieSpeciality.SPRINTER:
                                    position += new Vector3(0.0f, 1f, 0.0f);
                                    break;
                            }
                            vector3_2 = position - vector3_1;
                            float sqrMagnitude = vector3_2.sqrMagnitude;
                            if ((double)sqrMagnitude <= (double)num)
                            {
                                Vector3 vector3_3 = position - vector3_1;
                                float magnitude = vector3_3.magnitude;
                                Vector3 vector3_4 = vector3_3 / magnitude;
                                if (!((UnityEngine.Object)zombiesInRadiu != (UnityEngine.Object)targetZombie) || (double)Vector3.Dot(vector3_4, aimTransform.forward) >= 0.5)
                                {
                                    if ((double)magnitude > 0.025000000372529)
                                    {
                                        RaycastHit hit;
                                        PhysicsUtility.raycast(new Ray(vector3_1, vector3_4), out hit, magnitude - 0.025f, RayMasks.BLOCK_SENTRY, QueryTriggerInteraction.UseGlobal);
                                        if (!((UnityEngine.Object)hit.transform != (UnityEngine.Object)null) || !((UnityEngine.Object)hit.transform != (UnityEngine.Object)__instance.transform))
                                        {
                                            PhysicsUtility.raycast(new Ray(vector3_1 + vector3_4 * (magnitude - 0.025f), -vector3_4), out hit, magnitude - 0.025f, RayMasks.DAMAGE_SERVER, QueryTriggerInteraction.UseGlobal);
                                            if ((UnityEngine.Object)hit.transform != (UnityEngine.Object)null && (UnityEngine.Object)hit.transform != (UnityEngine.Object)__instance.transform)
                                                continue;
                                        }
                                        else
                                            continue;
                                    }
                                    num = sqrMagnitude;
                                    player = (Player)null;
                                    zombie = zombiesInRadiu;
                                }
                            }
                        }
                    }
                    animalsInRadius.Clear();
                    AnimalManager.getAnimalsInRadius(vector3_1, sqrRadius, animalsInRadius);
                    for (int index = 0; index < animalsInRadius.Count; ++index)
                    {
                        Animal animalsInRadiu = animalsInRadius[index];
                        if (!animalsInRadiu.isDead)
                        {
                            Vector3 position = animalsInRadiu.transform.position;
                            vector3_2 = position - vector3_1;
                            float sqrMagnitude = vector3_2.sqrMagnitude;
                            if ((double)sqrMagnitude <= (double)num)
                            {
                                Vector3 vector3_3 = position - vector3_1;
                                float magnitude = vector3_3.magnitude;
                                Vector3 vector3_4 = vector3_3 / magnitude;
                                if (!((UnityEngine.Object)animalsInRadiu != (UnityEngine.Object)targetAnimal) || (double)Vector3.Dot(vector3_4, aimTransform.forward) >= 0.5)
                                {
                                    if ((double)magnitude > 0.025000000372529)
                                    {
                                        RaycastHit hit;
                                        PhysicsUtility.raycast(new Ray(vector3_1, vector3_4), out hit, magnitude - 0.025f, RayMasks.BLOCK_SENTRY, QueryTriggerInteraction.UseGlobal);
                                        if (!((UnityEngine.Object)hit.transform != (UnityEngine.Object)null) || !((UnityEngine.Object)hit.transform != (UnityEngine.Object)__instance.transform))
                                        {
                                            PhysicsUtility.raycast(new Ray(vector3_1 + vector3_4 * (magnitude - 0.025f), -vector3_4), out hit, magnitude - 0.025f, RayMasks.DAMAGE_SERVER, QueryTriggerInteraction.UseGlobal);
                                            if ((UnityEngine.Object)hit.transform != (UnityEngine.Object)null && (UnityEngine.Object)hit.transform != (UnityEngine.Object)__instance.transform)
                                                continue;
                                        }
                                        else
                                            continue;
                                    }
                                    num = sqrMagnitude;
                                    player = (Player)null;
                                    zombie = (Zombie)null;
                                    animal = animalsInRadiu;
                                }
                            }
                        }
                    }
                    if ((UnityEngine.Object)player != (UnityEngine.Object)targetPlayer || (UnityEngine.Object)zombie != (UnityEngine.Object)targetZombie || (UnityEngine.Object)animal != (UnityEngine.Object)targetAnimal)
                    {
                        targetPlayerField.SetValue(__instance, player);
                        targetZombieField.SetValue(__instance, zombie);
                        targetAnimalField.SetValue(__instance, animal);
                        lastFireField.SetValue(__instance, Time.realtimeSinceStartup + 0.1f);
                    }
                }
                if ((UnityEngine.Object)targetPlayer != (UnityEngine.Object)null)
                {
                    switch (__instance.sentryMode)
                    {
                        case ESentryMode.NEUTRAL:
                        case ESentryMode.FRIENDLY:
                            isFiringField.SetValue(__instance, targetPlayer.animator.gesture != EPlayerGesture.SURRENDER_START);
                            break;
                        case ESentryMode.HOSTILE:
                            isFiringField.SetValue(__instance, true);
                            break;
                    }
                    isAimingField.SetValue(__instance, true);
                }
                else if ((UnityEngine.Object)targetZombie != (UnityEngine.Object)null)
                {
                    isFiringField.SetValue(__instance, true);
                    isAimingField.SetValue(__instance, true);
                }
                else if ((UnityEngine.Object)targetAnimal != (UnityEngine.Object)null)
                {
                    switch (__instance.sentryMode)
                    {
                        case ESentryMode.NEUTRAL:
                        case ESentryMode.FRIENDLY:
                            isFiringField.SetValue(__instance, targetAnimal.isHunting);
                            break;
                        case ESentryMode.HOSTILE:
                            isFiringField.SetValue(__instance, true);
                            break;
                    }
                    isAimingField.SetValue(__instance, true);
                }
                else
                {
                    isFiringField.SetValue(__instance, false);
                    isAimingField.SetValue(__instance, false);
                }
                if (isAiming && (double)Time.realtimeSinceStartup - (double)lastAim > (double)Provider.UPDATE_TIME)
                {
                    lastAimField.SetValue(__instance, Time.realtimeSinceStartup);
                    Transform transform = (Transform)null;
                    Vector3 vector3_3 = Vector3.zero;
                    if ((UnityEngine.Object)targetPlayer != (UnityEngine.Object)null)
                    {
                        transform = targetPlayer.transform;
                        vector3_3 = targetPlayer.look.aim.position;
                    }
                    else if ((UnityEngine.Object)targetZombie != (UnityEngine.Object)null)
                    {
                        transform = targetZombie.transform;
                        vector3_3 = targetZombie.transform.position;
                        switch (targetZombie.speciality)
                        {
                            case EZombieSpeciality.NORMAL:
                                vector3_3 += new Vector3(0.0f, 1.75f, 0.0f);
                                break;
                            case EZombieSpeciality.MEGA:
                                vector3_3 += new Vector3(0.0f, 2.625f, 0.0f);
                                break;
                            case EZombieSpeciality.CRAWLER:
                                vector3_3 += new Vector3(0.0f, 0.25f, 0.0f);
                                break;
                            case EZombieSpeciality.SPRINTER:
                                vector3_3 += new Vector3(0.0f, 1f, 0.0f);
                                break;
                        }
                    }
                    else if ((UnityEngine.Object)targetAnimal != (UnityEngine.Object)null)
                    {
                        transform = targetAnimal.transform;
                        vector3_3 = targetAnimal.transform.position + Vector3.up;
                    }
                    if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
                    {
                        float yaw = Mathf.Atan2(vector3_3.x - vector3_1.x, vector3_3.z - vector3_1.z) * 57.29578f;
                        double num = (double)vector3_3.y - (double)vector3_1.y;
                        vector3_2 = vector3_3 - vector3_1;
                        double magnitude = (double)vector3_2.magnitude;
                        float pitch = Mathf.Sin((float)(num / magnitude)) * 57.29578f;
                        BarricadeManager.sendAlertSentry(__instance.transform, yaw, pitch);
                    }
                }
                if (isFiring && hasWeapon && (__instance.displayItem.state[10] > (byte)0 && !__instance.isOpen) && (double)Time.realtimeSinceStartup - (double)lastFire > (double)fireTime)
                {
                    lastFireField.SetValue(__instance, lastFire + fireTime);
                    if ((double)Time.realtimeSinceStartup - (double)lastFire > (double)fireTime)
                        lastFire = Time.realtimeSinceStartup;
                    float num1 = (float)__instance.displayItem.quality / 100f;
                    if (attachments.magazineAsset == null)
                        return false;

                    if (!ownerPlayer.HasPermission(IgnoreAmmoPermission))
                    {
                        Console.WriteLine("Ammo reduction");
                        if ((__instance.sentryAsset.infiniteAmmo ? 1 : (((ItemGunAsset)displayAsset).infiniteAmmo ? 1 : 0)) == 0)
                            --__instance.displayItem.state[10];
                        
                        if (!__instance.sentryAsset.infiniteQuality && Provider.modeConfigData.Items.Has_Durability && (__instance.displayItem.quality > (byte)0 && (double)UnityEngine.Random.value < (double)((ItemWeaponAsset)displayAsset).durability))
                        {
                            if ((int)__instance.displayItem.quality > (int)((ItemWeaponAsset)displayAsset).wear)
                                __instance.displayItem.quality -= ((ItemWeaponAsset)displayAsset).wear;
                            else
                                __instance.displayItem.quality = (byte)0;
                        }
                    }
                    if (attachments.barrelAsset == null || !attachments.barrelAsset.isSilenced || __instance.displayItem.state[16] == (byte)0)
                        AlertTool.alert(__instance.transform.position, 48f);

                    float num2 = ((ItemGunAsset)displayAsset).spreadAim * ((double)num1 < 0.5 ? (float)(1.0 + (1.0 - (double)num1 * 2.0)) : 1f);
                    if (attachments.tacticalAsset != null && interact)
                        num2 *= attachments.tacticalAsset.spread;
                    if (attachments.gripAsset != null)
                        num2 *= attachments.gripAsset.spread;
                    if (attachments.barrelAsset != null)
                        num2 *= attachments.barrelAsset.spread;
                    if (attachments.magazineAsset != null)
                        num2 *= attachments.magazineAsset.spread;


                    if ((UnityEngine.Object)((ItemGunAsset)displayAsset).projectile == (UnityEngine.Object)null)
                    {
                        BarricadeManager.sendShootSentry(__instance.transform);
                        byte pellets = attachments.magazineAsset.pellets;
                        for (byte index1 = 0; (int)index1 < (int)pellets; ++index1)
                        {
                            EPlayerKill kill = EPlayerKill.NONE;
                            uint xp = 0;
                            float times = (float)(1.0 * ((double)num1 < 0.5 ? 0.5 + (double)num1 : 1.0));
                            Transform transform = (Transform)null;
                            float num3 = 0.0f;
                            if ((UnityEngine.Object)targetPlayer != (UnityEngine.Object)null)
                                transform = targetPlayer.transform;
                            else if ((UnityEngine.Object)targetZombie != (UnityEngine.Object)null)
                                transform = __instance.transform;
                            else if ((UnityEngine.Object)targetAnimal != (UnityEngine.Object)null)
                                transform = targetAnimal.transform;
                            if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
                            {
                                vector3_2 = transform.position - __instance.transform.position;
                                num3 = vector3_2.magnitude;
                            }
                            float num4 = (1f - num3 / ((ItemWeaponAsset)displayAsset).range) * (1f - ((ItemGunAsset)displayAsset).spreadHip) * 0.75f;
                            if ((UnityEngine.Object)transform == (UnityEngine.Object)null || (double)UnityEngine.Random.value > (double)num4)
                            {
                                Vector3 forward = aimTransform.forward;
                                forward += aimTransform.right * UnityEngine.Random.Range(-((ItemGunAsset)displayAsset).spreadHip, ((ItemGunAsset)displayAsset).spreadHip) * num2;
                                forward += aimTransform.up * UnityEngine.Random.Range(-((ItemGunAsset)displayAsset).spreadHip, ((ItemGunAsset)displayAsset).spreadHip) * num2;
                                forward.Normalize();
                                RaycastInfo raycastInfo = DamageTool.raycast(new Ray(aimTransform.position, forward), ((ItemWeaponAsset)displayAsset).range, RayMasks.DAMAGE_SERVER);
                                if (!((UnityEngine.Object)raycastInfo.transform == (UnityEngine.Object)null))
                                {
                                    DamageTool.impact(raycastInfo.point, raycastInfo.normal, raycastInfo.material, (UnityEngine.Object)raycastInfo.vehicle != (UnityEngine.Object)null || raycastInfo.transform.CompareTag("Barricade") || raycastInfo.transform.CompareTag("Structure") || raycastInfo.transform.CompareTag("Resource"));
                                    if ((UnityEngine.Object)raycastInfo.vehicle != (UnityEngine.Object)null)
                                        DamageTool.damage(raycastInfo.vehicle, false, Vector3.zero, false, ((ItemWeaponAsset)displayAsset).vehicleDamage, times, true, out kill, new CSteamID(), EDamageOrigin.Sentry);
                                    else if ((UnityEngine.Object)raycastInfo.transform != (UnityEngine.Object)null)
                                    {
                                        if (raycastInfo.transform.CompareTag("Barricade"))
                                        {
                                            ushort result;
                                            if (ushort.TryParse(raycastInfo.transform.name, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result))
                                            {
                                                ItemBarricadeAsset itemBarricadeAsset = (ItemBarricadeAsset)Assets.find(EAssetType.ITEM, result);
                                                if (itemBarricadeAsset != null && (itemBarricadeAsset.isVulnerable || ((ItemWeaponAsset)displayAsset).isInvulnerable))
                                                    DamageTool.damage(raycastInfo.transform, false, ((ItemWeaponAsset)displayAsset).barricadeDamage, times, out kill, new CSteamID(), EDamageOrigin.Sentry);
                                            }
                                        }
                                        else if (raycastInfo.transform.CompareTag("Structure"))
                                        {
                                            ushort result;
                                            if (ushort.TryParse(raycastInfo.transform.name, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result))
                                            {
                                                ItemStructureAsset itemStructureAsset = (ItemStructureAsset)Assets.find(EAssetType.ITEM, result);
                                                if (itemStructureAsset != null && (itemStructureAsset.isVulnerable || ((ItemWeaponAsset)displayAsset).isInvulnerable))
                                                    DamageTool.damage(raycastInfo.transform, false, raycastInfo.direction * Mathf.Ceil((float)attachments.magazineAsset.pellets / 2f), ((ItemWeaponAsset)displayAsset).structureDamage, times, out kill, new CSteamID(), EDamageOrigin.Sentry);
                                            }
                                        }
                                        else if (raycastInfo.transform.CompareTag("Resource"))
                                        {
                                            byte x;
                                            byte y;
                                            ushort index2;
                                            if (ResourceManager.tryGetRegion(raycastInfo.transform, out x, out y, out index2))
                                            {
                                                ResourceSpawnpoint resourceSpawnpoint = ResourceManager.getResourceSpawnpoint(x, y, index2);
                                                if (resourceSpawnpoint != null && !resourceSpawnpoint.isDead && ((ItemWeaponAsset)displayAsset).hasBladeID(resourceSpawnpoint.asset.bladeID))
                                                    DamageTool.damage(raycastInfo.transform, raycastInfo.direction * Mathf.Ceil((float)attachments.magazineAsset.pellets / 2f), ((ItemWeaponAsset)displayAsset).resourceDamage, times, 1f, out kill, out xp, new CSteamID(), EDamageOrigin.Sentry);
                                            }
                                        }
                                        else if (raycastInfo.section < byte.MaxValue)
                                        {
                                            InteractableObjectRubble componentInParent = raycastInfo.transform.GetComponentInParent<InteractableObjectRubble>();
                                            if ((UnityEngine.Object)componentInParent != (UnityEngine.Object)null && !componentInParent.isSectionDead(raycastInfo.section) && (componentInParent.asset.rubbleIsVulnerable || ((ItemWeaponAsset)displayAsset).isInvulnerable))
                                                DamageTool.damage(componentInParent.transform, raycastInfo.direction, raycastInfo.section, ((ItemWeaponAsset)displayAsset).objectDamage, times, out kill, out xp, new CSteamID(), EDamageOrigin.Sentry);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Vector3 point = Vector3.zero;
                                if ((UnityEngine.Object)targetPlayer != (UnityEngine.Object)null)
                                    point = targetPlayer.look.aim.position;
                                else if ((UnityEngine.Object)targetZombie != (UnityEngine.Object)null)
                                {
                                    point = targetZombie.transform.position;
                                    switch (targetZombie.speciality)
                                    {
                                        case EZombieSpeciality.NORMAL:
                                            point += new Vector3(0.0f, 1.75f, 0.0f);
                                            break;
                                        case EZombieSpeciality.MEGA:
                                            point += new Vector3(0.0f, 2.625f, 0.0f);
                                            break;
                                        case EZombieSpeciality.CRAWLER:
                                            point += new Vector3(0.0f, 0.25f, 0.0f);
                                            break;
                                        case EZombieSpeciality.SPRINTER:
                                            point += new Vector3(0.0f, 1f, 0.0f);
                                            break;
                                    }
                                }
                                else if ((UnityEngine.Object)targetAnimal != (UnityEngine.Object)null)
                                    point = targetAnimal.transform.position + Vector3.up;
                                DamageTool.impact(point, -aimTransform.forward, EPhysicsMaterial.FLESH_DYNAMIC, true);
                                Vector3 direction = aimTransform.forward * Mathf.Ceil((float)attachments.magazineAsset.pellets / 2f);
                                if ((UnityEngine.Object)targetPlayer != (UnityEngine.Object)null)
                                    DamageTool.damage(targetPlayer, EDeathCause.SENTRY, ELimb.SPINE, __instance.owner, direction, (IDamageMultiplier)((ItemWeaponAsset)displayAsset).playerDamageMultiplier, times, true, out kill, true, ERagdollEffect.NONE);
                                else if ((UnityEngine.Object)targetZombie != (UnityEngine.Object)null)
                                {
                                    IDamageMultiplier damageMultiplier = ((ItemWeaponAsset)displayAsset).zombieOrPlayerDamageMultiplier;
                                    DamageZombieParameters parameters = DamageZombieParameters.make(targetZombie, direction, damageMultiplier, ELimb.SPINE);
                                    parameters.times = times;
                                    parameters.legacyArmor = true;
                                    parameters.instigator = (object)__instance;
                                    DamageTool.damageZombie(parameters, out kill, out xp);
                                }
                                else if ((UnityEngine.Object)targetAnimal != (UnityEngine.Object)null)
                                {
                                    IDamageMultiplier damageMultiplier = ((ItemWeaponAsset)displayAsset).animalOrPlayerDamageMultiplier;
                                    DamageAnimalParameters parameters = DamageAnimalParameters.make(targetAnimal, direction, damageMultiplier, ELimb.SPINE);
                                    parameters.times = times;
                                    parameters.instigator = (object)__instance;
                                    DamageTool.damageAnimal(parameters, out kill, out xp);
                                }
                            }
                        }
                    }
                    __instance.rebuildState();
                }
            }
            return false;
        }
    }
}
