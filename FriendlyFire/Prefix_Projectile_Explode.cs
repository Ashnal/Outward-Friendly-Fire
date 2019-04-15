using Harmony;
using UnityEngine;

namespace FriendlyFire
{
    [HarmonyPatch(typeof(Projectile))]
    [HarmonyPatch("Explode")]
    public static class Prefix_Projectile_Explode
    {
        static FastInvokeHandler Projectile_OnProjectileHit;
        static FastInvokeHandler Projectile_EndLife;
        static void Prepare()
        {
            Projectile_OnProjectileHit = MethodInvoker.GetHandler(
                AccessTools.Method(typeof(Projectile), "OnProjectileHit")
            );
            Projectile_EndLife = MethodInvoker.GetHandler(
                AccessTools.Method(typeof(Projectile), "EndLife")
            );
        }

        public static bool ExplodePrefix(Projectile __instance, Collider _collider, Vector3 _hitPoint, Vector3 _hitDir, ref float ___m_lastHitTimer, ref float ___m_targetLightIntensity, 
            float ___m_lightStartIntensity, ref float ___m_lightIntensityFadeSpeed, SoundPlayer ___m_shootSound, SoundPlayer ___m_travelSound, ParticleSystem ___m_explosionFX, 
            ref object[] ___m_explodeInfos, Character.Factions[] ___m_targetableFactions)
        {
            UnityEngine.Debug.Log("FriendlyFire - ExplodePatch() Projectile: " + __instance.name);
            ___m_lastHitTimer = 5f;
            ___m_targetLightIntensity = ___m_lightStartIntensity * 1.8f;
            ___m_lightIntensityFadeSpeed = __instance.LightIntensityFade.x * 0.5f;
            if (___m_shootSound)
            {
                ___m_shootSound.Stop(false);
            }
            if (___m_travelSound)
            {
                ___m_travelSound.Stop(false);
            }
            Character character = null;
            if (_collider != null)
            {
                if (___m_explosionFX)
                {
                    ___m_explosionFX.Play();
                }
                character = _collider.GetCharacterOwner();
            }
            bool blocked = false;
            if (character != null)
            {
                //Debug.Log("FriendlyFire - ExplodePatch() instance.name = " + __instance.name);
                if (___m_targetableFactions.Contains(character.Faction))
                {
                    if (!__instance.Unblockable && character.ShieldEquipped)
                    {
                        Hitbox component = _collider.GetComponent<Hitbox>();
                        if (component != null && component.BlockBox && character.Blocking)
                        {
                            blocked = true;
                        }
                    }
                }
                else
                {
                    //Debug.Log("FriendlyFire - ExplodePatch() Caught Flamethrower");
                    character = null;
                }
            }

            Projectile_OnProjectileHit(__instance, new object[] { character, _hitPoint, _hitDir, blocked });
            ___m_explodeInfos[0] = character;
            ___m_explodeInfos[1] = _hitPoint;
            ___m_explodeInfos[2] = _hitDir;
            ___m_explodeInfos[3] = (_collider != null);
            __instance.SendMessage("OnExplodeDone", ___m_explodeInfos, SendMessageOptions.DontRequireReceiver);
            if (__instance.EndMode == Projectile.EndLifeMode.Normal || (__instance.EndMode == Projectile.EndLifeMode.EnvironmentOnly && (_collider == null || Global.FullEnvironmentMask == (Global.FullEnvironmentMask | 1 << _collider.gameObject.layer))))
            {
                Projectile_EndLife(__instance, new object[] { });
            }
            return false;
        }
    }
}
