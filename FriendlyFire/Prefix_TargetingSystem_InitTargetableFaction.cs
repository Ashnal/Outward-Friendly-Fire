using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace FriendlyFire
{
    [HarmonyPatch(typeof(TargetingSystem))]
    [HarmonyPatch("InitTargetableFaction")]
    public static class Prefix_TargetingSystem_InitTargetableFaction
    {
        static void Prepare()
        {
        }

        [HarmonyPrefix]
        public static bool InitTargetableFactionPrefix(TargetingSystem __instance, ref Vector3 ___m_realLockingPointOffset, ref int ___m_remainingHelpLockCount, Character ___m_character)
        {
            //prevent mosh pit
            //also it seems the ctor isn't running somehow? Hopefully this will fix it
            __instance.AlliedToSameFaction = true;
            __instance.Range = 20f;
            __instance.LongRange = 40f;
            __instance.HunterEyeRange = 80f;
            __instance.AdjustOffsetSpeed = 5f;
            __instance.OffsetModifier = 1f;
            __instance.LockingPointOffset = Vector3.zero;
            ___m_realLockingPointOffset = Vector3.zero;
            ___m_remainingHelpLockCount = 5;

            //UnityEngine.Debug.Log("FriendlyFire - InitTargetableFactionPatch( orig, " + instance.name + ")\r\n\tFaction = " + instance.m_character.Faction + "\r\n\tAlliedToSameFaction = " + instance.AlliedToSameFaction);
            List<Character.Factions> list = new List<Character.Factions>();
            for (int i = 1; i < 9; i++)
            {
                Character.Factions factions = (Character.Factions)i;
                if (!list.Contains(factions) && !__instance.StartAlliedFactions.Contains(factions) && (!__instance.AlliedToSameFaction || factions != ___m_character.Faction))
                {
                    //UnityEngine.Debug.Log("\tFriendlyFire - Adding " + factions + " to targetable factions for " + instance.m_character.name) ;
                    list.Add(factions);
                }
            }
            if (___m_character.Faction == Character.Factions.Player)
            {
                //UnityEngine.Debug.Log("FriendlyFire - Adding Player faction to targetable factions for " + instance.m_character.name);
                list.Add(Character.Factions.Player);
            }
            __instance.TargetableFactions = list.ToArray();
            return false;
        }
    }
}
