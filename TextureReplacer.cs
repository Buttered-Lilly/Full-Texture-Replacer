using MelonLoader;
using UnityEngine;
using System.Collections;
using MelonLoader.Utils;
using HarmonyLib;
using System.Data;

[assembly: MelonInfo(typeof(Full_Texture_Replacer.TextureReplacer), "Full Texture Replacer", "1.0.0", "Lilly", null)]

namespace Full_Texture_Replacer
{
    public class TextureReplacer : MelonMod
    {

        public override void OnLateInitializeMelon()
        {
            Replacer g = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube)).AddComponent<Replacer>();
            GameObject.DontDestroyOnLoad(g.gameObject);
        }

        public class Replacer : MonoBehaviour
        {
            public List<string> textures = new List<string>();
            public List<FileInfo> files = new List<FileInfo>();
            public string[] disableList;

            public void Start()
            {
                getFiles();
                StartCoroutine(list());
            }
            public void getFiles()
            {
                textures = new List<string>();
                files = new List<FileInfo>();
                DirectoryInfo dir = new DirectoryInfo(MelonEnvironment.UserDataDirectory + "/Skins");
                disableList = File.ReadAllLines(MelonEnvironment.UserDataDirectory + "/Skins/_DisableList.txt");
                foreach (var f in dir.EnumerateFiles())
                {
                    textures.Add(f.Name.Replace(".png", ""));
                    files.Add(f);
                    //MelonLogger.Msg(f.Name);
                }
            }

            void Update()
            {
                if (Input.GetKeyDown(KeyCode.Quote))
                {
                    MelonLogger.Msg("Reloading...");
                    getFiles();
                    if(!isRunning)
                        StartCoroutine(list());
                }
            }

            bool isRunning = false;

            public IEnumerator list()
            {
                int i = 0;
                int x = 1;
                int file = 0;

                //yield return Resources.LoadAll("", typeof(Renderer));

                Renderer[] loadedBundle = Resources.FindObjectsOfTypeAll(typeof(Renderer)) as Renderer[];
                yield return loadedBundle;

                //MelonLogger.Msg(loadedBundle.Length);

                isRunning = true;
                foreach (Renderer bundle in loadedBundle as Renderer[])
                {
                    x++;

                    foreach(string ob in disableList)
                    {

                        if (bundle.gameObject.name == ob)
                        {
                            try
                            {
                                bundle.enabled = false;
                            }
                            catch (Exception e)
                            {
                                MelonLogger.Msg(e);
                            }
                        }
                    }
                    foreach (var mat in bundle.sharedMaterials)
                    {
                        try
                        {
                            //MelonLogger.Msg("tex name: " + material.mainTexture);
                            string parsed = mat.mainTexture + "";
                            //MelonLogger.Msg(parsed);
                            parsed = parsed.Replace(" (UnityEngine.Texture2D)", "");
                            //MelonLogger.Msg(parsed);

                            file = textures.IndexOf(parsed);
                            if (file != -1)
                            {
                                Texture2D tex = mat.mainTexture as Texture2D;
                                //MelonLogger.Msg("Found: " + files[file].FullName);
                                tex.LoadImage(System.IO.File.ReadAllBytes(MelonEnvironment.UserDataDirectory + "/Skins/" + textures[file] + ".png"));
                            }
                        }
                        catch (Exception e)
                        {
                            //MelonLogger.Msg(e);
                        }
                        if (i > 20)
                        {
                            yield return new WaitForEndOfFrame();
                            i = 0;
                        }

                        i++;
                    }
                }
                //MelonLogger.Msg("Done Loading");
                isRunning = false;
                StartCoroutine(list());
            }
        }
    }
}