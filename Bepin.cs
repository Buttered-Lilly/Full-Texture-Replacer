using BepInEx;
using UnityEngine;
using HarmonyLib;


namespace Full_Texture_Replacer
{
    [BepInPlugin("LillysMods.Replacer", "Full Texture Replacer", "1.0.0.0")]
    public class Bepin : BaseUnityPlugin
    {
        Replacer replacer;
        private void Awake()
        {
            if (Replacer.replacerinstance != null)
                return;
            replacer = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube)).AddComponent<Replacer>();
            GameObject.DontDestroyOnLoad(replacer.gameObject);
            replacer.gameObject.hideFlags = HideFlags.HideAndDontSave;
            replacer.path = Info.Location.Replace("Full_Texture_Replacer.dll", "");
        }
    }
}
