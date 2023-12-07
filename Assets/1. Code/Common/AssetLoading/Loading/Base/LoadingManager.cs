//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine.Events;
//using UnityEngine.SceneManagement;
//using UnityEngine.ResourceManagement.ResourceProviders;

//namespace Common.AssetLoading
//{
//    public static class LoadingManager
//    {
//        public delegate void LoadOperation(UnityAction onComplete, LoadProgress loadProgress);
//        public static Dictionary<string, LoadOperation> LoadOperations { get; private set; } = new Dictionary<string, LoadOperation>();

//        public static LoadProgress CurrentOperation { get; private set; }

//        public static float CurrentLoadProgress
//        {
//            get
//            {
//                if (CurrentOperation.TotalPercentProgress != 0f)
//                    return CurrentOperation.TotalPercentProgress;
//                return 0f;
//            }
//        }



//        public static void BeginLoadingOperation(string id, UnityAction onComplete)
//        {
//            if (!LoadOperations.ContainsKey(id))
//                return;

//            Debug.Log("loading: " + id);

//            CurrentOperation = new LoadProgress(id);
//            LoadOperations[id].Invoke(onComplete, CurrentOperation);
//        }


//        public static LoadProgress BeginLoadingOperation(LoadOperation operation, string id, UnityAction onComplete)
//        {
//            Debug.Log("loading: " + id);

//            LoadProgress operationProgress = new LoadProgress(id);
//            operation.Invoke(onComplete, operationProgress);
//            return operationProgress;
//        }

//        public static LoadProgress BeginSceneLoading(string address, LoadSceneMode loadMode, UnityAction<SceneInstance> onComplete)
//        {
//            LoadProgress progress = new LoadProgress("sceneLoad_" + address);
//            LoadScene(address, loadMode, onComplete, progress);
//            return progress;
//        }




//        public static async Task LoadScene(string address, LoadSceneMode loadMode, UnityAction<SceneInstance> onComplete, LoadProgress progress)
//        {
//            progress.PercentProgress = 0f;
//            progress.CurrentItem = "loading scene";
//            await AddressableSceneLoader.LoadSceneTracked(address, onComplete, loadMode, progress);
//            progress.PercentProgress = 1f;
//            progress.Complete = true;
//        }











//    }
//}