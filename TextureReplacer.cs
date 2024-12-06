//using MelonLoader;
using UnityEngine;
using System.Collections;
//using MelonLoader.Utils;
using HarmonyLib;
using System.Data;
using UnityEngine.SceneManagement;


namespace Full_Texture_Replacer
{
    public class Replacer : MonoBehaviour
    {
        public static Replacer replacerinstance;
        public string path;
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
            DirectoryInfo dir = new DirectoryInfo(path + "/Skins");
            try
            {
                disableList = File.ReadAllLines(path + "/Skins/_DisableList.txt").ToList<string>();
            }
            catch { }
            foreach (var f in dir.EnumerateFiles())
            {
                textures.Add(f.Name.Replace(".png", ""));
                texturesFiles.Add(f);
                //MelonLogger.Msg(f.Name);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Quote))
            {
                Debug.Log("Reloading...");
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
                //MelonLogger.Msg(x + " / " + texx.Length);
                string parsed = t + "";
                parsed = parsed.Replace(" (UnityEngine.Texture2D)", "");
                file = textures.IndexOf(parsed);
                if (file != -1)
                {
                    Task<byte[]> tex = System.IO.File.ReadAllBytesAsync(path + "/Skins/" + textures[file] + ".png");
                    tex.Wait();
                    t.LoadImage(tex.Result);
                    //MelonLogger.Msg("Found: " + files[file].FullName);
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