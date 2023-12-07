using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Common.Utils.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type[] FindAllOfType<T>(this AppDomain domain, bool includeDeclaringType = false) 
        {
            Type type = typeof(T);

            List<Type> all = new List<Type>();

            foreach(Assembly assembly in domain.GetAssemblies())
                all.AddRange(assembly.GetTypes());

            List<Type> selected = new List<Type>();

            for(int i = 0; i < all.Count; i++)
            {
                if ((includeDeclaringType && all[i].DeclaringType == type) || all[i].IsSubclassOf(type))
                    selected.Add(all[i]);
            }

            return selected.ToArray();
        }

        public static Type[] FindAllOfInterface<T>(this AppDomain domain)
        {
            Type type = typeof(T);

            List<Type> all = new List<Type>();

            foreach(Assembly assembly in domain.GetAssemblies())
                all.AddRange(assembly.GetTypes());

            List<Type> selected = new List<Type>();

            return all.Where(t => t.GetInterfaces().Contains(type)).ToArray();
        }


        public static Type[] FindAllOfType<T>(this Assembly assembly) 
        {
            Type type = typeof(T);

            Type[] all = assembly.GetTypes();
            List<Type> selected = new List<Type>();

            for(int i = 0; i < all.Length; i++)
            {
                if (all[i].DeclaringType == type || all[i].IsSubclassOf(type))
                    selected.Add(all[i]);
            }

            return selected.ToArray();
        }

        public static Type[] FindAllOfType(this Assembly assembly, Type type)
        {
            Type[] all = assembly.GetTypes();
            List<Type> selected = new List<Type>();

            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].DeclaringType == type || all[i].IsSubclassOf(type))
                    selected.Add(all[i]);
            }

            return selected.ToArray();
        }

        public static Type[] FindAllOfInterface<T>(this Assembly assembly)
        {
            Type type = typeof(T);

            Type[] all = assembly.GetTypes();
            List<Type> selected = new List<Type>();

            return all.Where(t => t.GetInterfaces().Contains(type)).ToArray();
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if (type.BaseType == null) return type.GetInterfaces();

            return Enumerable.Repeat(type.BaseType, 1)
                             .Concat(type.GetInterfaces())
                             .Concat(type.GetInterfaces().SelectMany<Type, Type>(GetBaseTypes))
                             .Concat(type.BaseType.GetBaseTypes());
        }

        public static string WithoutExtension(this string path)
        {
            if(path.IndexOf('.') != -1)
                return path.Remove(path.IndexOf('.'));
            else
                return path;
        }

    }
}
