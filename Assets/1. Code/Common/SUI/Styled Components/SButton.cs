using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
#if UNITYEDITOR
using UnityEditor;
#endif
using TMPro;

namespace Common.SUI
{

    [AddComponentMenu("SUI/SButton")]
    [ExecuteInEditMode]
    public class SButton : StyledElement
    {
        public string text;
        
        private Button _button;
        private Image _image;
        private SText _text;

        [SerializeField]
        private ButtonElement button;


        protected override void OnUpdate()
        {
            UpdateButton();
            UpdateText();
        }

        private void UpdateButton()
        {
            if (!_button)
            {
                if (GetComponent<Button>())
                    _button = GetComponent<Button>();
                else
                    _button = gameObject.AddComponent<Button>();
            }

            if (!_image)
            {
                if (GetComponent<Image>())
                    _image = GetComponent<Image>();
                else
                    _image = gameObject.AddComponent<Image>();
            }

            if (!button.unique)
                if (theme)
                    button = theme.button;

            _button.targetGraphic = _image;
            _button.transition = Button.Transition.ColorTint;

            ColorBlock block = new ColorBlock
            {
                normalColor = button.normal.backgroundColor,
                highlightedColor = button.highlighted.backgroundColor,
                selectedColor = button.selected.backgroundColor,
                pressedColor = button.pressed.backgroundColor,
                colorMultiplier = 1
            };

            _button.colors = block;
        }

        private void UpdateText()
        {
            if(!_text)
            {
                if (!elementsContainer.Find("SButton_text"))
                {
                    GameObject obj = new GameObject("SButton_text");
                    obj.transform.SetParent(elementsContainer);
                    obj.AddComponent<RectTransform>();

                    _text = obj.AddComponent<SText>();
                }
                else
                    _text = elementsContainer.Find("SButton_text").GetComponent<SText>();
            }

            _text.textStyle = button.text;
            _text.panel = button.textPanel;

            _text.text = this.text;

            RectTransform r = (_text.transform as RectTransform);
            
            r.anchoredPosition = new Vector2(0, 0);
            r.anchorMin = new Vector2(0, 0);
            r.anchorMax = new Vector2(1, 1);

            r.sizeDelta = new Vector2(1, 1);
        }


        //public static void Create()
        //{
        //    GameObject obj = new GameObject("Button");
        //    obj.AddComponent<SButton>();

        //    if(UnityEditor.Selection.activeObject as GameObject != null)
        //        GameObjectUtility.SetParentAndAlign(obj, UnityEditor.Selection.activeObject as GameObject);

        //    Undo.RegisterCreatedObjectUndo(obj, "Button");

        //    UnityEditor.Selection.activeObject = obj;
        //}


    }

}