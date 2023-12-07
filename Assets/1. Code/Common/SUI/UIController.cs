using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.SUI
{
    public static class SUI
    {










    }





    public abstract class SRoot : MonoBehaviour
    {
        public Theme theme { get; protected set; }

        protected T Create<T>(string name = "styled element", Transform parent = null) where T : StyledElement
        {
            if (parent == null)
                parent = transform;

            T created = StyledElement.Create<T>(name, parent);

            created.theme = this.theme;

            return created;
        }

        private void Awake()
        {
            Init();
        }
        public abstract void Init();
    }





    public class UILayer
    {
        public bool Exclusive { get; protected set; }
        public bool EscapeToExit { get; protected set; }





    }
}
