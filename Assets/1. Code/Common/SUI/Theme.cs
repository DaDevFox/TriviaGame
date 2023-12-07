using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Common.SUI
{

    //TODO: Styled UI System

    [CreateAssetMenu(fileName = "New Theme", menuName = "UI/Theme")]
    public class Theme : ScriptableObject
    {
        public static Theme current { get; private set; }

        public RectElement rect;
        public ContainerElement container;

        public PanelElement panel;

        public TextElement text;

        public ButtonElement button;


        private void Awake()
        {
            current = this;
        }
    }

    /// <summary>
    /// Any class that is used as an information container regarding the style of a certain element of UI
    /// </summary>
    [Serializable]
    public abstract class ThemeElement 
        //: ScriptableObject
    {
        /// <summary>
        /// Used to detremine wether this ThemeElement is universal or specific to a single element
        /// <para>(only neccessary for ThemeElements used in SUIElements, not in the Theme)</para>
        /// </summary>
        public bool unique = false;


        public ThemeElement Clone(){
            return this.MemberwiseClone() as ThemeElement;
        }

        public T Clone<T>() where T : ThemeElement
        {
            return this.MemberwiseClone() as T;
        }

    }

    //[CreateAssetMenu(fileName = "Rect Style", menuName = "UI/Styles/Rect")]
    [Serializable]
    public class RectElement : ThemeElement
    {
        public int paddingLeft;
        public int paddingRight;
        public int paddingTop;
        public int paddingBottom;
    }

    //[CreateAssetMenu(fileName = "Container Style", menuName = "UI/Styles/Container")]
    [Serializable]
    public class ContainerElement : ThemeElement
    {
        public RectElement rectStyle;

        public int marginLeft;
        public int marginRight;
        public int marginTop;
        public int marginBottom;
    }



    //[CreateAssetMenu(fileName = "Panel Style", menuName = "UI/Styles/Panel")]
    [Serializable]
    public class PanelElement : ThemeElement
    {
        public ContainerElement container;

        public Color backgroundColor;
        public Color borderColor;

        public Sprite backgroundImage;

        public int borderWidthLeft;
        public int borderWidthRight;
        public int borderWidthTop;
        public int borderWidthBottom;
    }


    [Serializable]
    public class ImageElement : ThemeElement
    {
        public bool preserveAspect = true;
        
    }




    //[CreateAssetMenu(fileName = "Button Style", menuName = "UI/Styles/Button")]
    [Serializable]
    public class ButtonElement : ThemeElement
    {
        public PanelElement textPanel;
        public TextElement text;


        /// <summary>
        /// Modified fields on this object will apply for normal, highlighted, pressed, and selected
        /// </summary>
        public PanelElement inhereted;

        public PanelElement normal;
        public PanelElement highlighted;
        public PanelElement pressed;
        public PanelElement selected;
    }

    //[CreateAssetMenu(fileName = "Text Style", menuName = "UI/Styles/Text")]
    [Serializable]
    public class TextElement : ThemeElement
    {
        public TMP_FontAsset font;
        public Color textColor;

        public TextAlignmentOptions alignment;

        public float fontSize;
    }
}