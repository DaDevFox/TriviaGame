using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    [AddComponentMenu("UI/Progress Bar")]
    public class HorizontalProgressBar : MonoBehaviour
    {
        public RectTransform parent;

        public Image fill;
        /// <summary>
        /// Direction to fill in
        /// </summary>
        public Vector2 fillDirection = new Vector2(1, 0);

        /// <summary>
        /// The progress in percent from 0 to 1
        /// </summary>
        [Range(0f, 1f)]
        public float progress = 0f;
        private float _progress = 0f;

        public float transitionSpeed = 2f;

        void Update()
        {
            float x = fillDirection.x == 0 ? parent.sizeDelta.x : parent.sizeDelta.x * progress * fillDirection.x;
            float y = fillDirection.y == 0 ? parent.sizeDelta.y : parent.sizeDelta.y * progress * fillDirection.y;

            _progress = Mathf.Lerp(_progress, progress, Time.deltaTime * transitionSpeed);

            fill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parent.sizeDelta.x * _progress * fillDirection.x);
        }
    }
}
