using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Utils.Extensions
{
    public static class GameObjectExtensions
    {
        public static void ClearChildren(this GameObject obj)
        {
            if(obj == null)
                return;

            List<Transform> children = new List<Transform>();
            
            for(int i = 0; i < obj.transform.childCount; i++)
            {
                children.Add(obj.transform.GetChild(i));
            }

            foreach (Transform child in children)
                GameObject.Destroy(child.gameObject);
        }

        public static void ClearChildren(this Transform obj)
        {
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < obj.childCount; i++)
            {
                children.Add(obj.GetChild(i));
            }

            foreach (Transform child in children)
                GameObject.Destroy(child.gameObject);
        }

        /// <summary>
        /// Finds a child of the object by its gameobject hierarchical path
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Transform FindByPath(this Transform transform, string path){
            string[] parts = path.Split('/');
            Transform current = transform;
            foreach(string part in parts){
                if(current.Find(part) != null){
                    current = current.Find(part);
                }else
                    break;
            }

            return current;
        }

    }
}
