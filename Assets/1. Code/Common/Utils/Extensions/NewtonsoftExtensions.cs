using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace Common.Utils.Extensions
{
    public static class NewtonsoftExtensions
    {
        public static Color ValueAsColor(this JObject obj, string property)
        {
            if (!obj.ContainsKey(property))
                throw new KeyNotFoundException(property);

            Color c = new Color();
            c.FromHex(obj.Value<string>(property));

            return c;
        }
        public static Color ValueAsColor(this JToken obj, string property)
        {
            Color c = new Color();
            return c.CreateFromHex(obj.Value<string>(property));
        }

        public static Color ValueAsColor(this JToken obj, string property, float alpha)
        {
            Color c = new Color();
            c = c.CreateFromHex(obj.Value<string>(property));
            c.a = alpha;
            return c;
        }

        public static Vector3 AsVector3(this JToken token){
            return new Vector3((float)token["x"], (float)token["y"], (float)token["z"]);
        }

        public static Vector2 AsVector2(this JToken token)
        {
            return new Vector2((float)token["x"], (float)token["y"]);
        }

        public static T Read<T>(this JToken obj)
        {
            if (obj as JProperty == null)
                return default;

            return (obj as JProperty).Value.Value<T>();
        }

        public static T[] ReadArray<T>(this JToken obj)
        {
            return obj.Values<T>().ToArray();
        }

        /// <summary>
        /// Selects the token at the given path (as a JPath)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JToken Select(this JToken token, string path)
        {
            return JSONManager.Find(token, path);
        }

        /// <summary>
        /// Selects the value of the token at the given path (as a JPath)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Select<T>(this JToken token, string path)
        {
            return Select(token, path).Value<T>();
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}
