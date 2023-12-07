using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json;

namespace Common.Utils.Extensions
{
    public static class DeepCopyUtil
    {
        public static T DeepClone<T>(this T obj)
        {
            T newObj = (T)Activator.CreateInstance(typeof(T));

            FieldInfo[] fields = typeof(T).GetFields();

            foreach (FieldInfo field in fields)
            {
                typeof(T).GetField(field.Name)
                    .SetValue(
                    newObj,
                    typeof(T).GetField(field.Name)
                        .GetValue(obj));
            }


            return newObj;

            // // Don't serialize a null object, simply return the default for that object
            // if (ReferenceEquals(obj, null)) return default;

            // // initialize inner objects individually
            // // for example in default constructor some list property initialized with some values,
            // // but in 'source' these items are cleaned -
            // // without ObjectCreationHandling.Replace default constructor values will be added to result
            // var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            // return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), deserializeSettings);
        }
    }
}