using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.IO;
using Common.Assets;
using Common.Data.JSON.Converters;
using Common.Services;
using Common.Utils;
using Common.Utils.Extensions;
using System.Linq;


namespace Common.Data
{
    /// <summary>
    /// Requires constructor
    /// <para>
    /// Searches the data folder for certain JSON Objects (using the Newtonsoft JSON libraries) 
    /// and creates a variant of the base class this is attached to with different properties assigned (based on the json) and a unique id
    /// </para>
    /// <para>See ships and ship systems as an example</para>
    /// <para>Node: Data instances in JSON must be directly children of a file's root</para>
    /// </summary>
    public interface IDataInstance
    {
        //TODO: Clean up IDataInstance

        /// <summary>
        /// The properties to expect from JSON files
        /// </summary>
        Dictionary<string, Type> propertyTypes { get; }
        /// <summary>
        /// Gets assiged the values of properties once an instance is created
        /// </summary>
        JMetadata properties { get; }

        /// <summary>
        /// Any JObject with a 'template' field that matches this (within the directories if any are defined) will be selected and created as an instance
        /// </summary>
        string templateID { get; }
        /// <summary>
        /// Directories this data instance applies to, if left null, data instances will be created anywhere in the data directory
        /// </summary>
        string[] directories { get; }

        /// <summary>
        /// Unique ID per-instance; do not modify. If id is -1, instance is not from JSON; This can be used to find this data instance in the <seealso cref="Database"/> class
        /// </summary>
        int instanceID { get; set; }
        /// <summary>
        /// JSON Object used to create this data instance
        /// </summary>
        JToken source { get; set; }

        /// <summary>
        /// Use this instead of a constructor; sometimes an IDataInstance is created without being instanced from a registered json object, 
        /// </summary>
        void OnInstance();
    }

    /// <summary>
    /// Holds data extrapolated from JSON
    /// </summary>
    public static class Database
    {
        internal static Dictionary<string, List<IDataInstance>> data { get; set; } = new Dictionary<string, List<IDataInstance>>();
        internal static Dictionary<int, IDataInstance> indexed { get; set; } = new Dictionary<int, IDataInstance>();
        public static Dictionary<string, int> idIndexLookup { get; private set; } = new Dictionary<string, int>();

        internal static void Init(Dictionary<string, List<IDataInstance>> data)
        {
            Database.data = data;

            int index = 0;
            data.Values.ForEach((instances) => instances.ForEach((instance) =>
            {
                string _id = instance.source.SelectToken("id") as JProperty != null ? (string)((instance.source.SelectToken("id") as JProperty).Value) : null;
                indexed.Add(index, instance);

                if(_id != null && !idIndexLookup.ContainsKey(_id))
                    idIndexLookup.Add(_id, index);
                else if(_id != null)
                    Debug.LogWarning($"JSON-side id {_id} has multiple users, execution continuing using instanceID");
                instance.instanceID = index;
                index++;
            }));
        }

        /// <summary>
        /// Returns a copy of the data instance with the given instance id;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IDataInstance Get(int id) => indexed.ContainsKey(id) ? indexed[id].DeepClone() : null;

        /// <summary>
        /// Returns a copy of the data instance with the given JSON-side id (easier to use but less efficient);
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IDataInstance Get(string id)
        {
            return idIndexLookup.ContainsKey(id) ? Get(idIndexLookup[id]) : null;
        }

        /// <summary>
        /// Gets all Data Instances with the given id
        /// </summary>
        /// <param name="template">template id to search for</param>
        /// <returns></returns>
        public static IDataInstance[] GetFromTemplate(string template) => (data.ContainsKey(template) ? data[template] : new List<IDataInstance>()).ToArray();

        /// <summary>
        /// Gets all instances that extend a class <typeparamref name="T"/> and have the given id as their template id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <returns></returns>
        public static T[] GetFromTemplate<T>(string template) where T : class => (from instance in GetFromTemplate(template)
                                                                   where instance as T != null
                                                                   select instance as T).ToArray();

        /// <summary>
        /// Gets all instances that extend a class <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetAll<T>() where T : class
        {
            List<T> found = new List<T>();
            Debug.Log(data.Count);
            data.Values.ForEach(
                (instances) => instances.ForEach((instance) => {
                    if (instance == null)
                        return;
                    if (instance.GetType().IsSubclassOf(typeof(T))) 
                        found.Add(instance as T); }));
            return found.ToArray();
        }

        /// <summary>
        /// Gets all Data Instances
        /// </summary>
        /// <returns></returns>
        public static IDataInstance[] GetAll()
        {
            List<IDataInstance> all = new List<IDataInstance>();
            data.Values.ForEach((instances) => all.AddRange(instances));
            return all.ToArray();
        }

    }

    /// <summary>
    /// Extrapolates data from JSON and creates instances in Unity
    /// </summary>
    public class JSONManager : IService
    {
        public const string JSON_TEMPLATE_PROPERTY_NAME = "template";
        public const string JSON_ID_PROPERTY_NAME = "id";

        public static JSONManager main { get; } = new JSONManager();

        private static Dictionary<string, JObject> _files;
        public const char PATH_SEPERATOR = '/';

        #region JSON

        #region Parsing

        private static void Setup()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Vector3Converter());
        }

        private static void ParseAll()
        {
            _files = new Dictionary<string, JObject>();
            TextAsset[] found = Assets.Assets.LoadAllStreaming<TextAsset>("data", ".json");

            foreach (TextAsset asset in found)
                if (asset != null)
                    _files.Add(asset.name.Trim(), Parse(asset));
        }

        private static JObject Parse(TextAsset file)
        {
            string text = file.text;
            return JsonConvert.DeserializeObject(text) as JObject;
        }

        #endregion

        #region Access

        /// <summary>
        /// Fetches the JToken at the given property path, e.g. ships/heavy_cruisers/super_heavy_capital_ship_name
        /// <para>Note using a path will only work on this indexer and not on a JToken or any other Newtonsoft JSON indexers</para>
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public JToken this[string propertyPath]
        {
            get
            {
                if (!propertyPath.Contains(PATH_SEPERATOR))
                    return _files.ContainsKey(propertyPath.Trim()) ? _files[propertyPath] : null;
                else
                {
                    string[] pathParts = propertyPath.Split(PATH_SEPERATOR);
                    string fileName = pathParts[0];

                    JObject file = _files.ContainsKey(fileName.Trim()) ? _files[fileName] : null;
                    if (file == null)
                    {
                        Debug.LogError($"File not found [{fileName}]");
                        return null;
                    }

                    JToken last = file;
                    JToken current = null;
                    foreach (string part in pathParts)
                    {
                        if (part != fileName)
                        {
                            if (last[part] == null)
                                throw new InvalidOperationException($"path not found; {current} does not have child token with name {part}");

                            current = last[part];

                            last = current;
                        }
                    }

                    return current;
                }
            }
        }

        /// <summary>
        /// Loads a json file's root object at the given asset path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static JObject LoadAtFilePath(string filePath)
        {
            TextAsset file = Assets.Assets.Load<TextAsset>(filePath);
            if (file == null)
                Debug.LogError($"Could not load file at path {filePath}");

            return Parse(file);
        }

        /// <summary>
        /// Loads all JSON files's roots in the given directory
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static JObject[] LoadAllAtFilePath(string directoryPath)
        {
            TextAsset[] files = Assets.Assets.LoadAllStreaming<TextAsset>(directoryPath, new string[] { ".json" });
            List<JObject> result = new List<JObject>();
            foreach (TextAsset file in files)
                result.Add(Parse(file));

            return result.ToArray();
        }

        public static JToken Find(JToken @base, string path){
            string[] pathParts = path.Split(PATH_SEPERATOR);

            JToken last = @base;
            JToken current = null;
            foreach (string part in pathParts)
            {
                if (last[part] == null)
                    throw new InvalidOperationException($"path not found; {current} does not have child token with name {part}");

                current = last[part];

                last = current;
            }

            return current;
        }

        #endregion

        #endregion

        #region Data Instances

        private static void CreateInstances()
        {
            Debug.Log("Creating data instances");
            Dictionary<string, List<IDataInstance>> _instances = new Dictionary<string, List<IDataInstance>>();
            Dictionary<string, IDataInstance> _instanceSources = new Dictionary<string, IDataInstance>();

            Type[] instanceTypes = AppDomain.CurrentDomain.FindAllOfInterface<IDataInstance>();

            instanceTypes.ForEach((Type t) =>
            {
                if (t.ContainsGenericParameters)
                    return;
                
                if (t.IsAbstract)
                    return;
                
                IDataInstance obj = Activator.CreateInstance(t) as IDataInstance;

                if (!_instanceSources.ContainsKey(obj.templateID))
                    _instanceSources.Add(obj.templateID, obj);
                else
                    Debug.LogError($"Error: Data Instance template ID conflicts, {_instanceSources[obj.templateID].GetType().FullName} and {t.FullName} both have template ID {obj.templateID}");
            });


            _files.ForEach((pair) =>
               {
                   string path = pair.Key;
                   JObject root = pair.Value;

                   if (root == null)
                       return;


                   IEnumerable<JToken> found = root.Children();


                   //Debug.Log($"{pair.Key}:{root}");

                   found.ForEach((child) =>
                   {
                       JObject obj = child as JObject;
                       if (child is JProperty property)
                           obj = property.Value as JObject;

                       if (obj == null)
                           return;

                       if (obj[JSON_TEMPLATE_PROPERTY_NAME] != null)
                       {
                           string template = (string)obj[JSON_TEMPLATE_PROPERTY_NAME];

                           if (!_instances.ContainsKey(template))
                               _instances.Add(template, new List<IDataInstance>() { CreateFrom(obj, _instanceSources) });
                           else
                               _instances[template].Add(CreateFrom(obj, _instanceSources));
                       }
                   });

               });

            Debug.Log($"{_files.Count} data files loaded");
            Debug.Log($"{_instances.Count} instanced templates types");
            Database.Init(_instances);
        }

        private static IDataInstance CreateFrom(JToken data, Dictionary<string, IDataInstance> _instanceSources)
        {

            JContainer _base = GetDataBase(data);
            if (_base == null)
                return null;


            string template = _base.Value<string>(JSON_TEMPLATE_PROPERTY_NAME);

            Debug.Log($"creating instance {template}");

            if (template == null)
                return null;

            if (!_instanceSources.ContainsKey(template))
                return null;

            Type t = _instanceSources[template].GetType();

            IDataInstance obj = Activator.CreateInstance(t) as IDataInstance;

            obj.source = _base;

            if (_base.Value<string>("id") != null)
            {
                obj.properties.Add("id", _base.SelectToken("id"));
                // Debug.Log(_base.Value<string>("id"));
            }
            obj.propertyTypes.ForEach((pair) =>
            {
                if (_base.SelectToken(pair.Key) != null)
                    obj.properties.Set(pair.Key, _base.SelectToken(pair.Key));
            });

            obj.OnInstance();

            return obj;
        }

        private static JContainer GetDataBase(JToken token){
            if(token is JProperty && ((JProperty)token).Value is JContainer)
                return ((JProperty)token).Value as JContainer;
            else if(token as JContainer != null)
                return token as JContainer;
            return null;
        }

        #endregion

        #region Service Implementation

        public void OnInit()
        {
            Setup();
            ParseAll();
            CreateInstances();
        }

        public void OnDeregester()
        {
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void Awake()
        {
        }

        public void Start()
        {
        }

        #endregion

    }
}

namespace Common.Data.JSON.Converters
{
    public class Vector3Converter : CustomCreationConverter<Vector3>
    {
        public override Vector3 Create(Type objectType) => new Vector3();
    }
}