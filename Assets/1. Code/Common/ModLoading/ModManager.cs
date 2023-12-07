using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Common.Services;
using Common.Assets;

namespace Common.ModLoading
{
    public class ModManager : IService
    {
        public static string ModsDirectory { get; } = Path.Combine(Application.streamingAssetsPath, "mods");
        public static string ShadowDirectoryName { get; } = "shadow";
        public static string AssetBundleDirectoryName { get; } = "assets";
        public static string AssetBundleDataDirectoryName { get; } = "data";
        public static string CodeDirectoryName { get; } = "code";


        private void LoadAllMods()
        {
            string[] modDirs = Directory.GetDirectories(ModsDirectory);
            foreach(string modDir in modDirs)
            {
                Mod mod = new Mod(Path.GetFileNameWithoutExtension(modDir), modDir);

                //DONE: Mod Loading Setup

                #region Shadow Directory

                if (Directory.Exists(Path.Combine(modDir, ShadowDirectoryName)))
                {
                    //TODO: Shadow Directory; replace streaming assets with mod files at runtime, and revert after application ends
                    //TODO: Shadow Directory; cross mod compatability

                    string[] files = Directory.GetFiles(Path.Combine(modDir, ShadowDirectoryName));
                    string[] all = GetAllStreamingAssetPaths();

                    string[] replacedFiles =
                        (from file in files
                         where all.Contains(file)
                         select file).ToArray();

                    mod.files = files;
                }

                #endregion

                #region Assets Directory

                if (Directory.Exists(Path.Combine(modDir, AssetBundleDirectoryName)))
                {
                    foreach(string bundle in Directory.GetDirectories(Path.Combine(modDir, AssetBundleDirectoryName)))
                    {
                        //DONE: Mod Asset Loading
                        Assets.Assets.ImportAssetBundle(bundle, Path.GetFileNameWithoutExtension(modDir));

                        //foreach(string bundleVariant in Directory.GetDirectories(bundle))
                        //{
                        //    if (Path.GetFileNameWithoutExtension(bundleVariant) == AssetBundleDataDirectoryName)
                        //        continue;

                        //    Assets.Assets.ImportAssetBundle(bundleVariant, Path.GetFileNameWithoutExtension(modDir));
                        //}

                    }
                }

                #endregion

                #region Code Directory

                if (Directory.Exists(Path.Combine(modDir, CodeDirectoryName)))
                {
                    //TODO: Mod code compiling and execution
                    //TODO: Mod Logging


                }



                    #endregion

                }

            Debug.Log($"{modDirs.Length} mods loaded");
        }

        private string[] GetAllStreamingAssetPaths()
        {
            List<string> paths = new List<string>();
            foreach(string path in Directory.GetFiles(Application.streamingAssetsPath, "", SearchOption.AllDirectories))
            {
                paths.Add(path);
            }
            return paths.ToArray();
        }


        #region Service implementation

        public void Awake()
        {
            
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnDeregester()
        {
        }

        public void OnInit()
        {
            LoadAllMods();
        }

        public void Start()
        {
        }

        public void Update()
        {
        }

        #endregion

        public class Mod
        {
            public string Name { get; private set; }
            public string Path { get; private set; }
            public string[] files;



            public Mod(string name, string path)
            {
                Name = name;
                Path = path;
            }



        }

    }
}
