using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using TMPro;
using Common.Utils.Extensions;

namespace Common.SUI
{

    [AddComponentMenu("SUI/SText")]
    public class SText : SPanel
    {
        public string text;

        protected TextMeshProUGUI _text { get; private set; } = null;

        public TextElement textStyle;

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (!_text)
            {
                if (elementsContainer.Find("STextContainer_text"))
                    _text = elementsContainer.Find("STextContainer_text").GetComponent<TextMeshProUGUI>();
                else 
                {
                    GameObject obj = new GameObject("STextContainer_text");
                    obj.transform.SetParent(elementsContainer);

                    _text = obj.AddComponent<TextMeshProUGUI>();
                }

                Format();
            }


            if (!textStyle.unique)
                if (theme && textStyle != theme.text)
                    textStyle = theme.text;

            if(_text.text != text)
                _text.text = text;
            if(_text.color != textStyle.textColor)
                _text.color = textStyle.textColor;
            if (_text.font != textStyle.font)
                _text.font = textStyle.font;
            if (_text.alignment != textStyle.alignment)
                _text.alignment = textStyle.alignment;
            if(_text.fontSize != textStyle.fontSize)
                _text.fontSize = textStyle.fontSize;
        }  

        public void Format(){
            _text.color = textStyle.textColor;
            _text.font = textStyle.font;
            _text.alignment = textStyle.alignment;
            _text.fontSize = textStyle.fontSize;


            (_text.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            (_text.transform as RectTransform).anchorMin = new Vector2(0, 0);
            (_text.transform as RectTransform).anchorMax = new Vector2(1, 1);
            (_text.transform as RectTransform).offsetMin = new Vector2(base.panel.container.marginLeft, base.panel.container.marginBottom);
            (_text.transform as RectTransform).offsetMax = -new Vector2(base.panel.container.marginRight, base.panel.container.marginTop);
        }

    }

}