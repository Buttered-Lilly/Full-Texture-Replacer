using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

[assembly: MelonInfo(typeof(Full_Texture_Replacer.MelonLoad), "Full Texture Replacer", "1.0.0", "Lilly", null)]
[assembly: MelonOptionalDependencies("BepInEx")]

namespace Full_Texture_Replacer
{
    public class MelonLoad : MelonMod
    {
        Replacer replacer;
        public override void OnInitializeMelon()
        {
            if (Replacer.replacerinstance != null)
                return;
            replacer = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube)).AddComponent<Replacer>();
            replacer.gameObject.hideFlags = HideFlags.HideAndDontSave;
            replacer.path = MelonEnvironment.UserDataDirectory;
        }
    }
}
