using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace FriendlyFire
{
    [HarmonyPatch(typeof(ConeBlast))]
    [HarmonyPatch("Hit")]
    public static class Prefix_ConeBlast_Hit
    {
        static FastInvokeHandler ConeBlast_AffectHit;
        static void Prepare()
        {
            ConeBlast_AffectHit = MethodInvoker.GetHandler(
                AccessTools.Method(typeof(ConeBlast), "AffectHit")
            );
        }

        //Flamethrower self-damage fix
        public static bool HitPrefix(ConeBlast __instance, ref List<Hitbox> ___cachedHitBox)
        {
            //UnityEngine.Debug.Log("FriendlyFire - HitPatch() " + instance.name);
            RaycastHit[] array = Physics.SphereCastAll(__instance.transform.position, __instance.Radius, __instance.transform.forward, __instance.Range, Global.WeaponHittingMask);
            List<Character> list = new List<Character>();
            ___cachedHitBox.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                Hitbox component = array[i].collider.GetComponent<Hitbox>();
                if (component != null && __instance.OwnerCharacter.TargetingSystem.TargetableFactions != null && component.OwnerChar != __instance.OwnerCharacter && (!__instance.OnlyAlive || component.OwnerChar.Alive) && !list.Contains(component.OwnerChar) && __instance.ValidTarget(component.OwnerChar, array[i].point))
                {
                    ___cachedHitBox.Add(component);
                    list.Add(component.OwnerChar);
                }
            }
            ConeBlast_AffectHit(__instance, new object[] { ___cachedHitBox });
            return false;
        }
    }
}