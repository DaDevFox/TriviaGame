using UnityEngine;

namespace Common.SUI
{
    public class TestUI : SRoot
    {
        [SerializeField]
        private Theme _theme;
        
        public override void Init()
        {
            theme = _theme;

            SButton button = Create<SButton>("testButton");


            // Create panel with w100, h100, with 2 panels inside, spaced 5 from the inside of the parent panel and 10 away from eachother
        }
    }
}
