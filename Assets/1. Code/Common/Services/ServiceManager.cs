using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class ServiceManager : MonoBehaviour
    {
        /// <summary>
        /// Called after all services have initialized
        /// </summary>
        public static event Action PostInitialize;
        private static Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        public static void Register(Type type)
        {
            if (_services.ContainsKey(type))
                return;

            _services.Add(type, Activator.CreateInstance(type) as IService);
        }

        public static void Register<T>(T obj) where T : class, IService
        {
            Type type = typeof(T);

            if (_services.ContainsKey(type))
                return;

            _services.Add(type, obj);
        }

        public static void Register<T>() where T : class, IService
        {
            Type type = typeof(T);

            if (_services.ContainsKey(type))
                return;

            T instance = Activator.CreateInstance(type) as T;

            _services.Add(type, instance);
        }

        public static T GetService<T>() where T : class, IService
        {
            Type type = typeof(T);

            if (!_services.ContainsKey(type))
                return null;

            return _services[type] as T;
        }

        private void Init()
        {
            Type[] services = AppDomain.CurrentDomain.FindAllOfInterface<IService>();
            foreach (Type t in services)
                Register(t);
        }


        #region Service Methods

        void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
            Init();

            Debug.Log("Services Registered");
            foreach (IService service in _services.Values)
                service.Awake();
        }

        void Start()
        {
            foreach (IService service in _services.Values)
                service.Start();
            foreach (IService service in _services.Values)
                service.OnInit();
            Debug.Log("Services Initialized");
            PostInitialize?.Invoke();

        }

        void Update()
        {
            foreach (IService service in _services.Values)
                service.Update();
        }

        void LateUpdate()
        {
            foreach (IService service in _services.Values)
                service.LateUpdate();
        }

        void FixedUpdate()
        {
            foreach (IService service in _services.Values)
                service.FixedUpdate();
        }

        #endregion

    }
}
