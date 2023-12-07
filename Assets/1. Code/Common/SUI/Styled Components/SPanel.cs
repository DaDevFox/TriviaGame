using Common.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Common.SUI.Utils;

namespace Common.SUI.Utils{
    public static class SUIExtensions{
        public static T Clone<T>(this T element) where T : ThemeElement{
            return element.Clone<T>();
        }
    }
}


namespace Common.SUI
{



    [AddComponentMenu("SUI/SPanel")]
    public class SPanel : StyledElement
    {
        [Tooltip("Whether or not the styler will constantly override any external style changes I.E. a button or another script modifying its subject every frame. ")]
        public bool _override = true;

        protected Image _image { get; private set; }

        /// <summary>
        /// Panel Element to determine the look of the panel; Leave null to use theme default
        /// </summary>
        [Tooltip("Panel Element to determine the look of the panel; Leave null to use theme default")]
        public PanelElement panel = null;

        protected override void OnStart()
        {
            
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            UpdateImage();
            UpdateChildren();
        }

        private void UpdateImage()
        {
            if (!_image)
            {
                if (!GetComponent<Image>())
                    _image = gameObject.AddComponent<Image>();
                else
                    _image = GetComponent<Image>();

                if (!panel.unique)
                    if (theme && panel != theme.panel)
                        panel = theme.panel.DeepClone();

                // if(theme.panel != panel)
                //     panel.unique = true;

                _image.sprite = null;
                _image.color = panel.backgroundColor;

                _image.sprite = panel.backgroundImage;
            }


            if (!panel.unique)
                if (theme && panel != theme.panel)
                    panel = theme.panel.DeepClone();

            if(_override){
                if(_image.color != panel.backgroundColor)
                    _image.color = panel.backgroundColor;
                if (_image.sprite != panel.backgroundImage)
                    _image.sprite = panel.backgroundImage;
            }

            //if (!GetComponent<AspectRatioFitter>())
            //{
            //    var fitter = gameObject.AddComponent<AspectRatioFitter>();

            //    fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            //    fitter.aspectRatio = 0.001f;
            //}


        }

        private void UpdateChildren()
        {
            //for(int i = 0; i < transform.childCount; i++)
            //{
            //    Transform child = transform.GetChild(i);    
            //    if (child.name == "SUI")
            //        continue;

            //    if(child.transform as RectTransform == null)
            //        child.gameObject.AddComponent<RectTransform>();

            //    RectTransform r = child.transform as RectTransform;

            //    r.anchorMin = new Vector2(0, 0);
            //    r.anchorMax = new Vector2(1, 1);

            //    r.offsetMin = new Vector2(panel.container.marginLeft, panel.container.marginBottom);
            //    r.offsetMax = -new Vector2(panel.container.marginRight, panel.container.marginTop);
            //}
        }


    }

}
