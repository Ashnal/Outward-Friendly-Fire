using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Harmony;

namespace FriendlyFire
{
    [HarmonyPatch(typeof(TargetingSystem))]
    [HarmonyPatch("IsTargetable")]
    public static class Prefix_TargetingSystem_IsTargetable
    {
        static void Prepare()
        {
        }

        public static bool IsTargetablePrefix(TargetingSystem __instance, ref bool __result, ref Character ___m_character, Character _character)
        {
            //checks the stack to see if this was called from certain functions that need to have different behavior
            bool checkFrame(StackFrame f)
            {
                System.Reflection.MethodBase method = f.GetMethod();
                //UnityEngine.Debug.Log("Method.Name checked: " + method.Name);
                return method.Name.Contains("CheckIfCombatWorthy") || method.Name.Contains("IsEnemyClose") || method.DeclaringType == typeof(TargetingSystem) && !method.Name.Contains("IsTargetable");
            }

            UnityEngine.Debug.Log("FriendlyFire - IsTargetablePatch() Stacktrace: \r\n" + Environment.StackTrace);
            //UnityEngine.Debug.Log("FriendlyFire - IsTargetablePatch() " + _character);
            //foreach (Character c in instance.m_character.EngagedCharacters)
            //{
            //    UnityEngine.Debug.Log("Engaged: " + c);
            //}

            StackTrace st = new StackTrace();
            IEnumerable<StackFrame> frames = from StackFrame sf in st.GetFrames()
                                             where checkFrame(sf)
                                             select sf;

            //any stackframes that met the initial conditions
            if (frames.Any())
            {
                //UnityEngine.Debug.Log("FriendlyFire - IsTargetablePatch() Stacktrace: \r\n" + Environment.StackTrace);
                //UnityEngine.Debug.Log("FriendlyFire - Found this frame: " + frames);
                if (_character.Faction == Character.Factions.Player &&
                    (_character == ___m_character || !ControlsInput.Sprint(___m_character.OwnerPlayerSys.PlayerID) ||
                    //fixes resting and combat music
                    frames.Any(x => x.GetMethod().Name == "IsEnemyClose" || frames.Any(y => y.GetMethod().Name == "CheckIfCombatWorthy"))))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
