//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Unity.Plastic.Newtonsoft.Json;
//using Unity.Plastic.Newtonsoft.Json.Linq;


//namespace Common.Utils
//{

//    /// <summary>
//    /// Stores shared data that any reference to the object can access, modify, and add
//    /// <para>Useful for ships and shipsystems, the information system, etc. </para>
//    /// </summary>
//    public class Metadata : IEnumerable
//    {
//        public int count => _data.Count;

//        public Dictionary<string, object> _data = new Dictionary<string, object>();

//        public object this[string key]
//        {
//            get
//            {
//                return Get(key);
//            }
//            set
//            {
//                Set(key, value);
//            }
//        }

//        public virtual void Set(string key, object value)
//        {
//            if (_data.ContainsKey(key))
//                _data[key] = value;
//            else
//                _data.Add(key, value);
//        }

//        public virtual void Add(string key, object value) => _data.Add(key, value);

//        public virtual void Clear() => _data.Clear();

//        public virtual object Get(string key) => _data.ContainsKey(key) ? _data[key] : null;

//        public virtual T Get<T>(string key) => Get(key) != null ? (T)Get(key) : default(T);



//        public IEnumerator GetEnumerator()
//        {
//            return ((IEnumerable)_data).GetEnumerator();
//        }
//    }

//    /// <summary>
//    /// Special <seealso cref="Metadata"/> that only allows JTokens to be stored and their values to be read and modified
//    /// </summary>
//    public class JMetadata : Metadata
//    {
//        public override void Add(string key, object value)
//        {
//            if(!value.GetType().IsSubclassOf(typeof(JToken)))
//                throw new ArgumentException("Value must be of type JToken");
//            base.Add(key, value);
//        }

//        /// <summary>
//        /// Attempts to get a value from a JToken in the metadata's data at the specified key. Returns the boxed value of the JToken as an object
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public override object Get(string key)
//        {
//            JToken token = ((_data.ContainsKey(key) && _data[key] is JToken ? (JToken)_data[key] : null));
//            if (token == null)
//                return GetDirect(key);
//            return token.Value<object>();
//        }

//        public override T Get<T>(string key)
//        {
//            if (base.Get<JToken>(key) == null)
//                return GetDirect<T>(key);
//            return base.Get<JToken>(key).Value<T>();
//        }

//        public T GetDirect<T>(string key) => Get(key) != null ? (T)Get(key) : default(T);
//        public object GetDirect(string key) => _data.ContainsKey(key) ? _data[key] : null;

//        public override void Set(string key, object value)
//        {
//            if (!value.GetType().IsSubclassOf(typeof(JToken)))
//                throw new ArgumentException("Value must be of type JToken");
//            base.Set(key, value);
//        }

//        public void SetDirect(string key, object value){
//            Debug.Log(base._data.Count);

//            if (base._data.ContainsKey(key))
//                base._data[key] = value;
//            else
//                base._data.Add(key, value);

//            Debug.Log(base._data.Count);
//        }
//    }
//}