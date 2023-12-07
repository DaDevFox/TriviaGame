using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Reflection;
using UnityEditor;
using Common.Utils.Extensions;
using Object = UnityEngine.Object;

namespace Common.Assets
{
    public class Assets
    {
        public static Assets instance { get; } = new Assets();

        private static List<StreamingAssetImporter> _importers;

        public static string ResourcesPath { get; } = "Assets/Resources/";
        public static string StreamingAssetsProjectPath => Application.isEditor ? "Assets/StreamingAssets/" : $"{Application.productName}_Data/StreamingAssets/";

        public static string ResourcesTag { get; } = "res:";
        public static string StreamingAssetsTag { get; } = "str:";
        public static string ModAssetsTag { get; } = "mod:";




        private static Dictionary<string, Object> _modAssets = new Dictionary<string, Object>();
        

        private Assets()
        {
            InitImporters();
        }


        #region Asset Path Checkers

        /// <summary>
        /// Checks if an asset from the resource folder exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsResourcePath(string path)
        {
            if (path.Contains(ResourcesPath))
                path = path.Replace(ResourcesPath, "");

            Object result = Resources.Load(path);
            bool exists = result != null;
            // Resources.UnloadAsset(result);

            return exists || path.Trim().StartsWith(ResourcesTag);
        }

        /// <summary>
        /// Checks if an asset from the streaming assets directory exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsStreamingAssetPath(string path)
        {
            if (path.Contains(StreamingAssetsProjectPath))
                path = path.Replace(StreamingAssetsProjectPath, "");

            Object asset = ImportStreamingAsset<Object>(Path.Combine(StreamingAssetsProjectPath, path));


            return asset != null || path.Trim().StartsWith(StreamingAssetsTag);
        }

        /// <summary>
        /// Checks if an asset with the given path has been registered by a mod(s)' AssetBundles
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsModAssetPath(string path)
        {
            return _modAssets.ContainsKey(path) || path.Trim().StartsWith(ModAssetsTag);
        }

        #endregion

        #region Loading Assets

        /// <summary>
        /// Loads all assets in the resources directory in unity object form with one of the given file extensions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static T[] LoadAllResources<T>(string path, string[] extensions = null) where T : UnityEngine.Object
        {
            List<T> found = new List<T>();

            if (path.Contains(ResourcesPath))
                path.Replace(ResourcesPath, "");

            string fullPath = Path.Combine(ResourcesPath, path);


            foreach(string filePath in Directory.GetFiles(fullPath).Where(
                (_path) =>
                {
                    string ext = Path.GetExtension(_path);
                    if (extensions == null)
                        return true;
                    else
                        return extensions.Contains(ext);
                }))
            {
                T asset = LoadResource<T>(filePath);
                if (!asset)
                    continue;
                found.Add(asset);
            }

            return found.ToArray();
        }

        /// <summary>
        /// Loads all streaming assets in unity object form with one of the given file extensions
        /// </summary>
        /// <typeparam name="T">type of the unity object streaming assets will be converted to</typeparam>
        /// <param name="path">the path inside of the streaming assets folder</param>
        /// <param name="extensions">the file extensions to search for</param>
        /// <returns></returns>
        public static T[] LoadAllStreaming<T>(string path, params string[] extensions) where T : UnityEngine.Object
        {
            List<T> found = new List<T>();

            if (path.Contains(StreamingAssetsProjectPath))
                path.Replace(StreamingAssetsProjectPath, "");

            string fullPath = Path.Combine(StreamingAssetsProjectPath, path);

            foreach (string filePath in Directory.EnumerateFiles(fullPath, "*", SearchOption.AllDirectories).Where(
                (_path) =>
                {
                    string ext = Path.GetExtension(_path);

                    if (extensions.Length == 0)
                        return true;
                    else
                        return extensions.Contains(ext);
                }))
            {
                T asset = ImportStreamingAsset<T>(filePath);
                if (!asset)
                    continue;
                found.Add(asset);
            }

            return found.ToArray();
        }

        /// <summary>
        /// Loads an asset from either the resources directory, streaming assets, or a mod's asset bundle depending on the path
        /// 
        /// <para>
        /// a path can have one of the following three prefixes:
        /// </para>
        /// <list type="">
        /// <item><c>res:</c></item>
        /// <item><c>str:</c></item>
        /// <item><c>mod:</c></item>
        /// </list>
        /// each will respectively pull from
        /// Resources,
        /// StreamingAssets, and
        /// Mod AssetBundles
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            if (IsResourcePath(path))
                return LoadResource<T>(path);
            if (IsStreamingAssetPath(path))
                return LoadStreaming<T>(path);
            if (IsModAssetPath(path))
                return LoadMod<T>(path);

            return null;
        }


        /// <summary>
        /// Loads an asset from the resources directory and converts it into Unity object form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadResource<T>(string resourcePath) where T : UnityEngine.Object
        {
            string path = resourcePath;
            if(path.Contains(ResourcesPath))
                path = path.Replace(ResourcesPath,"");

            if (path.Contains(ResourcesTag))
                path = path.Replace(ResourcesTag, "");

            path = path.WithoutExtension();


            T asset = Resources.Load<T>(path);
            if(asset == null)
                Debug.LogError($"Could not load asset of type {typeof(T).Name} at resource path [{path}].");

            return asset;
        }

        /// <summary>
        /// Attempts to load a streaming asset from the streamingassets directory and tries to import it as a Unity object
        /// <para>Note that not all filetypes support importing, and importing might fail if no importer for the filetype is found</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadStreaming<T>(string streamingPath) where T : UnityEngine.Object
        {
            string path = streamingPath;
            if(path.Contains(StreamingAssetsProjectPath))
                path = path.Replace(StreamingAssetsProjectPath,"");

            if (path.Contains(StreamingAssetsTag))
                path = path.Replace(StreamingAssetsTag, "");

            T asset = ImportStreamingAsset<T>(Path.Combine(StreamingAssetsProjectPath, path));
            if(asset == null)
                Debug.LogError($"Could not load streaming asset of type {typeof(T).Name} at path [{Path.Combine(StreamingAssetsProjectPath, path)}].");

            return asset;
        }

        /// <summary>
        /// Loads a mod asset from its AssetBundle with the following path format:
        /// <para> <code>  [name of mod directory]/[scene path of asset excluding 'assets'] </code>  </para>
        /// not case sensitive
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadMod<T>(string path) where T : UnityEngine.Object
        {
            if (path.Contains(ModAssetsTag))
                path = path.Replace(ModAssetsTag, "");

            path = path.ToLower();
            if (!_modAssets.ContainsKey(path))
            {
                Debug.LogError($"No mod asset at path {path} registered");
                return null;
            }

            return _modAssets[path] as T;
        }

        #endregion

        #region Streaming Asset Importers

        private static T ImportStreamingAsset<T>(string path) where T : UnityEngine.Object
        {
            string extension = Path.GetExtension(path);
            StreamingAssetImporter importer = GetImporterFor(extension);
            if(importer == null)
            {
                Debug.LogError($"Could not find importer for extension {extension}");
                return null;
            }

            return importer.Import<T>(path);
        }

        private static StreamingAssetImporter GetImporterFor(string fileExtension)
        {
            foreach(StreamingAssetImporter importer in _importers)
            {
                if(importer.supportedFileTypes.Contains(fileExtension))
                    return importer;
            }
            return null;
        }

        private static void InitImporters()
        {
            _importers = new List<StreamingAssetImporter>();

            Type[] types = AppDomain.CurrentDomain.FindAllOfType<StreamingAssetImporter>();
            foreach(Type t in types)
            {
                StreamingAssetImporter importer = Activator.CreateInstance(t) as StreamingAssetImporter; 
                
                if(importer != null)
                    _importers.Add(importer);
            }
            
        }

        #endregion

        #region AssetBundles

        public static void ImportAssetBundle(string bundlePath, string name)
        {
            string platform = "win64";
            if (Application.platform == RuntimePlatform.LinuxPlayer)
            {
                platform = "linux";
            }
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                platform = "osx";
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (SystemInfo.operatingSystem.Contains("64"))
                {
                    platform = "win64";
                }
                else
                {
                    platform = "win32";
                }
            }

            //NOTE: Variants not supported for mod AssetBundle loading

            string manifestPath = Path.Combine(bundlePath, ModLoading.ModManager.AssetBundleDataDirectoryName, platform, platform);

            AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestPath);
            AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string _bundleName = manifest.GetAllAssetBundles()[0];
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(bundlePath, ModLoading.ModManager.AssetBundleDataDirectoryName, platform, _bundleName));
            
            ImportAssetBundle(bundle, name);
        }

        public static void ImportAssetBundle(AssetBundle bundle, string name)
        {
            string[] paths = bundle.GetAllAssetNames();

            foreach(string p in paths)
            {
                string path = p;
                List<string> parts = path.Split(new char[] { '/', '\\' }).ToList();
                parts.RemoveAt(0);

                string newPath = "";
                parts.ForEach((s) => { newPath = Path.Combine(newPath, s); });

                path = newPath;
                path = Path.Combine(name, path);
                path = path.Replace(@"\", "/");

                _modAssets.Add(path.ToLower(), bundle.LoadAsset(p)); ;
            }
        }







        #endregion

    }



    public abstract class StreamingAssetImporter
    {
        public string[] supportedFileTypes { get; protected set; }

        public abstract T Import<T>(string path) where T : UnityEngine.Object;
    }

    public class ImageImporter : StreamingAssetImporter
    {
        public ImageImporter()
        {
            supportedFileTypes = new string[] {".png"};
        }

        public override T Import<T>(string path) 
        {
            Texture2D result = null;
            string extension = Path.GetExtension(path);
            if(extension == ".png")
                result = ImportPNG(path);

            return result as T;
        }

        private Texture2D ImportPNG(string path)
        {
            Texture2D texture = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false);
            return texture;
        }

    }

    public class TextAssetImporter : StreamingAssetImporter
    {
        public TextAssetImporter()
        {
            supportedFileTypes = new string[] { ".txt", ".json", ".xml", ".md" };
        }

        public override T Import<T>(string path)
        {
            string text = File.ReadAllText(path);
            TextAsset asset = new TextAsset(text)
            {
                name = Path.GetFileNameWithoutExtension(path)
            };

            return asset as T;
        }
    }

    //public class MeshImporter : StreamingAssetImporter
    //{
    //    public MeshImporter()
    //    {
    //        supportedFileTypes = new string[] { ".fbx" };
    //    }

    //}


}


