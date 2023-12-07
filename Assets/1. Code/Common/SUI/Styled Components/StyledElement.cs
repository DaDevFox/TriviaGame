using UnityEngine;

namespace Common.SUI
{
    /// <summary>
    /// Any Styled UI Element
    /// (Creates a child called SUI, do not mess with it)
    /// </summary>
    [ExecuteInEditMode]
    public abstract class StyledElement : MonoBehaviour
    {
        public Theme theme;
        protected Transform elementsContainer { get; private set; }

        public static T Create<T>(string name = "styled element", Transform parent = null) where T : StyledElement
        {
            GameObject obj = new GameObject(name);
            if(parent)
                obj.transform.SetParent(parent);

            return obj.AddComponent<T>();
        }

        #region GameObject methods

        private void Start()
        {
            if (transform.GetComponentInParent<StyledElement>() != null)
                theme = transform.GetComponentInParent<StyledElement>().theme;



            OnStart();
        }
        
        private void Update()
        {
            if (transform.GetComponentInParent<StyledElement>() != null)
                theme = transform.GetComponentInParent<StyledElement>().theme;

            //Debug.Log($"{name}:{transform.GetComponentInParent<StyledElement>()}");

            if (elementsContainer == null)
            {
                if (!transform.Find("SUI"))
                {
                    elementsContainer = new GameObject("SUI").transform;
                    elementsContainer.transform.SetParent(transform);
                    elementsContainer.gameObject.AddComponent<RectTransform>();
                }
                else
                    elementsContainer = transform.Find("SUI");

                (elementsContainer.transform as RectTransform).anchorMin = new Vector2(0, 0);
                (elementsContainer.transform as RectTransform).anchorMax = new Vector2(1, 1);
                (elementsContainer.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
                (elementsContainer.transform as RectTransform).sizeDelta = new Vector2(1, 1);
            }

            OnUpdate();
        }

        private void LateUpdate()
        {
            OnLateUpdate();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnLateUpdate()
        {

        }

        protected virtual void OnFixedUpdate()
        {

        }

        #endregion

    }

}
