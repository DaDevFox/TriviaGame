//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;
//using Common.AssetLoading;
//using TMPro;

//namespace Common.UI
//{
//    public class LoadingScreenBase : MonoBehaviour
//    {
//        public event EventHandler OnActivate;
//        public event EventHandler OnDeactivate;
        
//        public LoadProgress OperationProgress { get; private set; } 

//        [SerializeField]
//        private Vector2 progressBarScaling = new Vector2(1f,0f);
//        [SerializeField]
//        private ProgressBar progressBar;
//        [SerializeField]
//        private TextMeshProUGUI statusText;

//        public bool Loading { get; private set; } = false;

//        public void Start()
//        {
//        }

//        public void Activate(LoadProgress operation)
//        {
//            this.gameObject.SetActive(true);
//            this.Loading = true;

//            //this.OnActivate.Invoke(this, EventArgs.Empty);

//            this.OperationProgress = operation;
//            this.OperationProgress.onComplete += OnLoadingFinish;
//        }

//        public void Deactivate()
//        {
//            this.Loading = false;
//            this.gameObject.SetActive(false);

//            //this.OnDeactivate.Invoke(this, EventArgs.Empty);
//        }


//        void Update()
//        {
//            if (!Loading)
//            {
//                if (progressBar)
//                    progressBar.progress = 1f;
//                if (statusText)
//                    statusText.text = "";

//                return;
//            }

//            if (progressBar != null)
//            {
//                float progress = OperationProgress.TotalPercentProgress != 0f ? OperationProgress.TotalPercentProgress : 0f;
//                progressBar.progress = progress;
//            }

//            if(statusText != null)
//                statusText.text = OperationProgress.TieredCurrentItem;
            
//        }

//        void OnLoadingFinish(object sender, EventArgs e)
//        {
//            this.Deactivate();
//        }
//    }
//}
