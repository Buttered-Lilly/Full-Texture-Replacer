using MelonLoader;
using UnityEngine;
using System.Collections;
using MelonLoader.Utils;
using HarmonyLib;
using System.Data;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(Full_Texture_Replacer.TextureReplacer), "Full Texture Replacer", "1.0.0", "Lilly", null)]

namespace Full_Texture_Replacer
{
    public class TextureReplacer : MelonMod
    {
        Replacer replacer;

        public override void OnLateInitializeMelon()
        {
            replacer = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube)).AddComponent<Replacer>();
            GameObject.DontDestroyOnLoad(replacer.gameObject);
            replacer.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        public class Replacer : MonoBehaviour
        {
            public List<string> textures = new List<string>();
            public List<string> disableList = new List<string>();
            public List<FileInfo> texturesFiles = new List<FileInfo>();

            public void Start()
            {
                getFiles();
                //getMeshFiles();
                StartCoroutine(Texture());
                SceneManager.sceneLoaded += run;
            }

            public void getFiles()
            {
                textures = new List<string>();
                texturesFiles = new List<FileInfo>();
                DirectoryInfo dir = new DirectoryInfo(MelonEnvironment.UserDataDirectory + "/Skins");
                try
                {
                    disableList = File.ReadAllLines(MelonEnvironment.UserDataDirectory + "/Skins/_DisableList.txt").ToList<string>();
                }
                catch { }
                foreach (var f in dir.EnumerateFiles())
                {
                    textures.Add(f.Name.Replace(".png", ""));
                    texturesFiles.Add(f);
                    //MelonLogger.Msg(f.Name);
                }
            }

            /*List<AssetBundle> bundles = new List<AssetBundle>();

            public IEnumerator getMeshFiles()
            {
                texturesFiles = new List<FileInfo>();
                DirectoryInfo dir = new DirectoryInfo(MelonEnvironment.UserDataDirectory + "/Meshs");
                foreach (var f in dir.EnumerateFiles())
                {
                    var bundleRequest = AssetBundle.LoadFromFileAsync(f.FullName);
                    yield return bundleRequest;
                    bundles.Add(bundleRequest.assetBundle);
                }
                foreach (AssetBundle b in bundles)
                {
                    try
                    {
                        foreach(string item in b.GetAllAssetNames())
                        {
                            GameObject.Find(item);
                        }
                    }
                    catch(Exception e)
                    {
                        MelonLogger.Msg(e);
                    }
                }
            }*/

            void Update()
            {
                if (Input.GetKeyDown(KeyCode.Quote))
                {
                    MelonLogger.Msg("Reloading...");
                    getFiles();
                    StartCoroutine(Texture());
                }
                StartCoroutine(Renders());
            }

            public void run(Scene scene, LoadSceneMode mode)
            {
                StartCoroutine(Renders());
                StartCoroutine(Texture());
            }

            public IEnumerator Renders()
            {
                int i = 0;
                foreach (string g in disableList)
                {
                    try
                    {
                        GameObject x = GameObject.Find(g);
                        x.GetComponent<Renderer>().enabled = false;
                        //MelonLogger.Msg("Disabled: " + g);
                    }
                    catch (Exception e)
                    {
                        //MelonLogger.Msg(e);
                    }
                    if (i > 5)
                    {
                        yield return new WaitForEndOfFrame();
                        i = 0;
                    }
                    i++;
                }
                if (i > 5)
                {
                    yield return new WaitForEndOfFrame();
                    i = 0;
                }
                i++;
            }

            public IEnumerator Texture()
            {
                int i = 0;
                int file = 0;

                Texture2D[] texx = Resources.FindObjectsOfTypeAll(typeof(Texture2D)) as Texture2D[];
                yield return texx;
                foreach (Texture2D t in texx)
                {
                    try
                    {
                        //MelonLogger.Msg(x + " / " + texx.Length);
                        string parsed = t + "";
                        parsed = parsed.Replace(" (UnityEngine.Texture2D)", "");
                        file = textures.IndexOf(parsed);
                        if (file != -1)
                        {
                            t.LoadImage(System.IO.File.ReadAllBytes(MelonEnvironment.UserDataDirectory + "/Skins/" + textures[file] + ".png"));
                            //MelonLogger.Msg("Found: " + files[file].FullName);
                        }
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Msg(e);
                    }
                    if (i > 20)
                    {
                        yield return new WaitForEndOfFrame();
                        i = 0;
                    }
                    i++;
                }
            }
        }
    }
}