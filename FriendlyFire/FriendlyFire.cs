using Partiality.Modloader;
using Harmony;
using System.Reflection;

namespace FriendlyFire
{
    public class FriendlyFire : PartialityMod
    {
        public FriendlyFire()
        {
            ModID = "Friendly Fire";
            Version = "0.9.1";
            author = "Ashnal";
        }

        public override void Init()
        {
            var harmony = HarmonyInstance.Create("com.ashnal.outward.extendedquickslots");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}