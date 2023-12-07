//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Common.AssetLoading;
//using UnityEngine.ResourceManagement.ResourceProviders;
//using UnityEngine.SceneManagement;
//using UnityEngine.Events;


//namespace Common.UI
//{
//    [RequireComponent(typeof(Button))]
//    public class LoadSceneOperationButton : MonoBehaviour
//    {
//        Button m_button;

//        [SerializeField]
//        private string sceneToLoad;
//        [SerializeField]
//        private LoadSceneMode loadSceneMode = LoadSceneMode.Single;
//        [SerializeField]
//        private LoadingScreenBase loadingScreen;

//        void Start()
//        {
//            m_button = gameObject.GetComponent<Button>();
//            m_button.onClick.AddListener(OnButtonClick);
//        }

//        void OnButtonClick()
//        {
//            if (sceneToLoad != string.Empty)
//            {
//                LoadProgress operationProgress = LoadingManager.BeginSceneLoading(sceneToLoad, loadSceneMode, OnLoaded);
//                if (this.loadingScreen != null)
//                {
//                    this.loadingScreen.Activate(operationProgress);
//                }
//            }
//        }

//        void OnLoaded(SceneInstance scene)
//        {
//            Debug.Log("scene " + sceneToLoad + " has been loaded");
//        }

//    }

//}