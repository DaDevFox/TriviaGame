//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine.ResourceManagement.AsyncOperations;

//namespace Common.AssetLoading
//{
//    public class LoadProgress
//    {
//        public event EventHandler onComplete;
//        public AsyncOperationHandle OperationHandle { get; private set; }
//        public bool hasHandle { get; private set; } = false;

//        private LoadProgress parent;
//        private List<LoadProgress> children = new List<LoadProgress>();

//        private bool complete;
//        public bool Complete 
//        {
//            set
//            {
//                if (value == true)
//                    onComplete.Invoke(this, EventArgs.Empty);
//                complete = value;
//            }
//            get
//            {
//                return complete;
//            }
//        }

//        public string OperationId { get; private set; }

//        public string TieredCurrentItem
//        {
//            get
//            {
//                string item = null;
//                foreach(LoadProgress child in children)
//                {
//                    if(child.CurrentItem != null)
//                    {
//                        item = child.CurrentItem;
//                    }
//                }
//                if(item != null)
//                    return this.CurrentItem + ": " + item;
//                else
//                    return this.CurrentItem;
//            }
//        }
//        public string CurrentItem { get; set; }
//        public float TotalPercentProgress 
//        {
//            get 
//            {
//                if (children.Count > 0)
//                {
//                    float progress = 0f;
//                    foreach (LoadProgress child in children)
//                    {
//                        progress += child.PercentProgress;
//                    }

//                    return progress / children.Count;
//                }
//                else
//                    return PercentProgress;
//            }
//        }

//        private float percentProgress;
//        public float PercentProgress 
//        {
//            get
//            {
//                if (hasHandle)
//                    return OperationHandle.PercentComplete;
//                else
//                    return percentProgress;
//            }
//            set
//            {
//                percentProgress = value;
//            }

//        }
        

//        public LoadProgress(string operationId)
//        {
//            this.OperationId = operationId;
//            this.Complete = false;
//        }

//        public LoadProgress(LoadProgress parent, string operationId)
//        {
//            this.parent = parent;
//            this.OperationId = operationId;
//            parent.children.Add(this);
//            this.Complete = false;
//        }

//        public void LinkToHandle(AsyncOperationHandle handle)
//        {
//            OperationHandle = handle;
//            hasHandle = true;
//        }

//    }
//}
