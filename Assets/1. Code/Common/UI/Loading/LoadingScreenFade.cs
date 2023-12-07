//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using System.Collections.Generic;
//using Common.UI;

//namespace Common.UI
//{

//    [RequireComponent(typeof(LoadingScreenBase))]
//    public class LoadingScreenFade : MonoBehaviour
//    {
//        public float fadeSpeed = 0.1f;
//        private float desiredFade = 0f;

//        private float currentFade = 0f;

//        LoadingScreenBase m_loadingScreen;
//        [SerializeField]
//        private List<Image> imagesToFade;


//        void Start()
//        {
//            m_loadingScreen = GetComponent<LoadingScreenBase>();
//        }

//        void Update()
//        {
//            desiredFade = m_loadingScreen.Loading ? 1f : 0f;

//            float difference = desiredFade - currentFade;
//            float c_difference = Mathf.Clamp(difference, -fadeSpeed, fadeSpeed);

//            currentFade += c_difference * Time.unscaledDeltaTime;

//            foreach (Image image in imagesToFade)
//            {
//                image.color = new Color(image.color.r, image.color.g, image.color.b, currentFade);
//            }
//        }
//    }
//}